using System.Collections.Concurrent;
using Mahjong.Core;

namespace Mahjong.Resolution.SearchState;

/// <summary>
/// 多线程版本,并行,
/// todo 待测试
/// </summary>
public class SearchStateVParallel : ISearchLogic
{
    private static readonly ConcurrentDictionary<string, bool> VisitedStates = new();
    private static readonly SemaphoreSlim Semaphore = new(10); // 限制并发数
    private static volatile bool SolutionFound = false;
    private GameLogic initialState;
    LinkedList<GameLogic> rootPath = new LinkedList<GameLogic>();

    public void Initialize(GameLogic initialState)
    {
        this.initialState = initialState;
        rootPath.AddLast(initialState);
    }

    public async Task<LinkedList<GameLogic>?> SearchState()
    {
        var cts = new CancellationTokenSource();
        
        try
        {
            var solution = await Task.Run(() => InternalSearchStateParallel(initialState, rootPath, cts.Token));
            return solution;
        }
        finally
        {
            cts.Dispose();
        }
    }
    
    
    private LinkedList<GameLogic> InternalSearchStateParallel(GameLogic initialState, 
        LinkedList<GameLogic> initialPath, CancellationToken cancellationToken)
    {
        var parallelOptions = new ParallelOptions
        {
            CancellationToken = cancellationToken,
            MaxDegreeOfParallelism = Environment.ProcessorCount
        };

        var solution = new ConcurrentStack<LinkedList<GameLogic>>();
        
        // 将初始状态的所有子节点分配给不同线程
        var initialMoves = SearchTool.GeneratePossibleMoves(initialState);
        
        Parallel.ForEach(initialMoves, parallelOptions, move =>
        {
            if (SolutionFound || cancellationToken.IsCancellationRequested)
                return;

            var nextLogic = new GameLogic(initialState.GameBoard.DeepClone());
            nextLogic.MergeAction(move.StartPos, move.EndPos, move.Offset, move.Distance, move.Direction);
            
            var newPath = new LinkedList<GameLogic>();
            newPath.AddLast(initialState);
            newPath.AddLast(nextLogic);

            var result = SearchStateIterative(newPath, cancellationToken);
            if (result != null)
            {
                solution.Push(result);
                SolutionFound = true;
                // 这里需要取消
                // cancellationToken.Cancel();
            }
        });

        return solution.TryPop(out var foundSolution) ? foundSolution : null;
    }

    private LinkedList<GameLogic> SearchStateIterative(LinkedList<GameLogic> path, 
        CancellationToken cancellationToken)
    {
        var stack = new Stack<LinkedList<GameLogic>>();
        stack.Push(new LinkedList<GameLogic>(path));

        while (stack.Count > 0 && !SolutionFound && !cancellationToken.IsCancellationRequested)
        {
            var currentPath = stack.Pop();
            var current = currentPath.Last();

            if (current.IsFinalState())
            {
                return currentPath;
            }

            string stateKey = current.GetStateHash();
            if (VisitedStates.ContainsKey(stateKey))
            {
                continue;
            }

            var nextMoves = SearchTool.GeneratePossibleMoves(current);
            
            foreach (var move in nextMoves)
            {
                if (SolutionFound || cancellationToken.IsCancellationRequested)
                    break;

                var nextLogic = new GameLogic(current.GameBoard.DeepClone());
                nextLogic.MergeAction(move.StartPos, move.EndPos, move.Offset, move.Distance, move.Direction);
                
                var newPath = new LinkedList<GameLogic>(currentPath);
                newPath.AddLast(nextLogic);
                
                stack.Push(newPath);
            }

            VisitedStates.TryAdd(stateKey, true);
        }

        return null;
    }
}