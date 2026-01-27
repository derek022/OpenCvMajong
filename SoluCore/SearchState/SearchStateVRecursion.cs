using Serilog;
using SoluCore.Game;

namespace SoluCore.SearchState;

/// <summary>
/// 递归版本
/// </summary>
public class SearchStateVRecursion : ISearchLogic
{
    protected readonly ILogger Logger = Log.ForContext<SearchStateVRecursion>();
    private static bool SolutionFound = false;
    
    LinkedList<IGameLogic> initialPath = new();
    
    /// <summary>
    /// 死局状态,用于剔除多余计算
    /// </summary>
    LinkedList<IGameLogic> clipStates = new();
    
    public void Initialize(IGameLogic initialState)
    {
        SolutionFound = false;
        
        initialPath.AddLast(initialState);
    }

    public Task<LinkedList<IGameLogic>?> SearchState()
    {
        if (InternalSearchState(initialPath))
        {
            return Task.FromResult(initialPath);
        }

        return null;
    }

    /// <summary>
    /// 剪枝和重复状态判断
    /// </summary>
    /// <param name="input"></param>
    /// <param name="next"></param>
    /// <returns></returns>
    public bool IsProcessedState(LinkedList<IGameLogic>? inputs, IGameLogic next)
    {

        foreach (var input in inputs)
        {

            if (next.Stage.IsSameState(input.Stage))
            {
                return true;
            }
        }

        foreach (var deadState in clipStates)
        {
            if (next.Stage.IsSameState(deadState.Stage))
            {
                return true;
            }
        }

        return false;
    }

    public bool IsMatchDead( IGameLogic next)
    {
        if (next.Stage.IsSameState(next))
        {
            return true;
        }

        return false;
    }


    private bool InternalSearchState(LinkedList<IGameLogic> states)
    {
        if (states.Count == 0)
        {
            Logger.Error("没有找到解题步骤");
            return false;
        }
        var current = states.Last();
        if (current.IsFinalState())
        {
            Logger.Error(" find solution path");
            SolutionFound = true;
            return true;
        }
        
        // Logger.Error("--------------- 开始搜索当前牌局------------");
        current.PrintState();
        foreach (var action in current.GetActions())
        {
            SearchStateOnAction(states, current, action);
            if (SolutionFound)
            {
                return true;
            }
        }
        
        if (!IsMatchDead(current))
        {
            clipStates.AddLast(current);
        }
        
        Thread.Sleep(10);
        return false;
    }
    
    
    
    
    private void SearchStateOnAction(LinkedList<IGameLogic> states, IGameLogic current, IAction action )
    {
        if (current.IsCanDoAction(action))
        {
            IGameLogic next = current.DeepClone();
            bool doAction = next.DoAction(action);

            if (doAction && !IsProcessedState(states, next))
            {
                states.AddLast(next);
                if (InternalSearchState(states))
                {
                    return;
                }
                states.RemoveLast();   
            }
        }
    }

}