using Mahjong.Core;
using Mahjong.Core.Util;
using Serilog;

namespace Mahjong.Resolution.SearchState;

/// <summary>
/// 递归版本
/// </summary>
public class SearchStateV1Recursion : ISearchLogic
{
    private static bool Finished = false;
    
    LinkedList<GameLogic> initialPath = new();
    private static List<GameLogic> DeadStates = new();
    
    public void Initialize(GameLogic initialState)
    {
        Finished = false;
        
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
    
    
    private static bool InternalSearchState(LinkedList<GameLogic> states)
    {
        var current = states.Last();
        if (current.IsFinalState())
        {
            Finished = true;
            return true;
        }

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

                    // todo 没有进行剪枝，有很多重复的逻辑
                    var from = values[i];
                    var to = values[j];

                    // 第一步，同一个行列，不用执行两边。
                    foreach (var isVer in SearchTool.GetSearchDirArr(from, to))
                    {
                        SearchStateOnAction(states, current, from, to, isVer);
                        if (Finished)
                        {
                            return true;
                        }
                    }
                }
            }
        }

        current.PrintState();
        Log.Information("当前状态没有发现有效路径。返回上一步。。");
        Thread.Sleep(10);
        DeadStates.Add(current);
        return false;
    }
    
    private static bool IsProcessedState(GameLogic current)
    {
        foreach (var state in DeadStates)
        {
            if (state.IsSameState(current))
            {
                return true;
            }
        }
        return false;
    }
    
    private static void SearchStateOnAction(LinkedList<GameLogic> states, GameLogic current, Vector2Int from, Vector2Int to,
        bool isVer)
    {
        if (current.CanMergeAction(from, to, isVer, out var offset, out var distance))
        {
            GameLogic next = new GameLogic(current.GameBoard.DeepClone());
            next.MergeAction(from, to, offset, distance, Tools.GetDir(from, to, isVer));
            if (!IsProcessedState(next))
            {
                states.AddLast(next);
                if (InternalSearchState(states))
                {
                    return;
                }
                states.RemoveLast();
            }
            else
            {
                Log.Error("当前状态已经处理过。。。");
            }
        }
    }

}