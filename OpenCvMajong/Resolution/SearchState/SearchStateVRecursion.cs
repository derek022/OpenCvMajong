using Mahjong.Core;
using Mahjong.Core.Util;
using Serilog;

namespace Mahjong.Resolution.SearchState;

/// <summary>
/// 递归版本
/// </summary>
public class SearchStateVRecursion : ISearchLogic
{
    protected readonly ILogger Logger = Serilog.Log.ForContext<SearchStateVRecursion>();
    private static bool SolutionFound = false;
    
    LinkedList<GameLogic> initialPath = new();
    
    /// <summary>
    /// 死局状态,用于剔除多余计算
    /// </summary>
    LinkedList<GameLogic> deadStates = new();
    
    public void Initialize(GameLogic initialState)
    {
        SolutionFound = false;
        
        initialPath.AddLast(initialState);
    }

    public Task<LinkedList<GameLogic>> SearchState()
    {
        if (InternalSearchState(initialPath))
        {
            return Task.FromResult(initialPath);
        }

        return null!;
    }
    
    
    private bool InternalSearchState(LinkedList<GameLogic> states)
    {
        var current = states.Last();
        if (current.IsFinalState())
        {
            Logger.Error(" find final state");
            SolutionFound = true;
            return true;
        }
        
        // 剪枝，已经判断死局的状态中，包含当前状态，直接返回。
        if (IsMatchDead(current))
        {
            // 符合死局条件
            Logger.Error("已经发现当前符合死局条件，直接返回上一步，不在搜索");
            return false;
        }
        
        Logger.Error("--------------- 开始搜索当前牌局------------");
        current.PrintState();
        foreach (var pair in current.CardPositions)
        {
            var values = pair.Value;
            for (int i = 0; i < values.Count; i++)
            {
                for (var j = 0; j < values.Count; j++)
                {
                    if (i == j)
                    {
                        continue;
                    }

                    var from = values[i];
                    var to = values[j];

                    // 第一步，同一个行列，不用执行两边。
                    foreach (var isVer in SearchTool.GetSearchDirArr(from, to))
                    {
                        SearchStateOnAction(states, current, from, to, isVer);
                        if (SolutionFound)
                        {
                            return true;
                        }
                    }
                }
            }
        }

        deadStates.AddLast(current);
        Thread.Sleep(10);
        return false;
    }
    
    
    private void SearchStateOnAction(LinkedList<GameLogic> states, GameLogic current, Vector2Int from, Vector2Int to,
        bool isVer)
    {
        if (current.CanMergeAction(from, to, isVer, out var offset, out var distance))
        {
            GameLogic next = new GameLogic(current.GameBoard.DeepClone());
            next.MergeAction(from, to, offset, distance, Tools.GetDir(from, to, isVer));
            states.AddLast(next);
            if (InternalSearchState(states))
            {
                return;
            }

            states.RemoveLast();
        }
    }

    private bool IsMatchDead(GameLogic current)
    {
        foreach (var deadState in deadStates)
        {
            if (SearchTool.ArrayMatchesWithWildcard(current.GameBoard.Boards, deadState.GameBoard.Boards, Cards.Zero))
            {
                return true;
            }
        }

        return false;
    }

}