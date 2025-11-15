using System.Collections.Concurrent;
using Mahjong.Core;

namespace Mahjong.Resolution.SearchState;

/// <summary>
/// 非递归版本
/// </summary>
public class SearchStateV2NonRecursion : ISearchLogic
{
    
    Stack<(GameLogic,LinkedList<GameLogic>)> stateStack = new Stack<(GameLogic logic, LinkedList<GameLogic> path)>();
        
    private static readonly ConcurrentDictionary<string, bool> VisitedStates = new();

    
    // 使用栈替代递归，避免栈溢出
    public void Initialize(GameLogic initialState)
    {
        var initialPath = new LinkedList<GameLogic>();
        initialPath.AddLast(initialState);
        stateStack.Push((initialState, initialPath));
    }

    public Task<LinkedList<GameLogic>> SearchState()
    {
        return Task.FromResult(SearchStateIterative(stateStack));
    }

    private static LinkedList<GameLogic> SearchStateIterative(Stack<(GameLogic logic, LinkedList<GameLogic> path)> stack)
    {
        while (stack.Count > 0)
        {
            var (current, path) = stack.Pop();
            
            if (current.IsFinalState())
            {
                return path;
            }

            // 检查是否已访问过该状态
            string stateKey = current.GetStateHash(); // 需要在GameLogic中实现
            if (VisitedStates.ContainsKey(stateKey))
            {
                continue;
            }

            // 生成所有可能的下一步
            var nextMoves = SearchTool.GeneratePossibleMoves(current);
            
            foreach (var move in nextMoves)
            {
                var nextLogic = new GameLogic(current.GameBoard.DeepClone());
                nextLogic.MergeAction(move.StartPos, move.EndPos, move.Offset, move.Distance, move.Direction);
                
                var newPath = new LinkedList<GameLogic>(path);
                newPath.AddLast(nextLogic);
                
                stack.Push((nextLogic, newPath));
            }

            // 标记当前状态已访问
            VisitedStates.TryAdd(stateKey, true);
        }

        return null;
    }

}