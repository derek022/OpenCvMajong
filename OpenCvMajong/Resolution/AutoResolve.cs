using Mahjong.Core;
using Mahjong.Core.Util;
using Serilog;

namespace Mahjong.Resolution;

public class AutoResolve
{
    private static bool Finished = false;
    public static void Init(Cards[,] initBoard)
    {
        Finished = false;
        var board = new GameBoard();
        board.SetBoardData(initBoard);
        
        GameLogic logic = new GameLogic(board);
        
        LinkedList<GameLogic> states = new();
        states.AddLast(logic);
        SearchState(states);
    }
    
    public static bool SearchState(LinkedList<GameLogic> states)
    {
        var current = states.Last();
        if (current.IsFinalState())
        {
            Finished = true;
            PrintResults(states);
            return true;
        }

        foreach (var pair in current.CardPositions)
        {
            var values = pair.Value;
            for (int i = 0; i < values.Count; i++)
            {
                for (var j = 0; j < values.Count; j++)
                {
                    if (i != j)
                    {
                        var from = values[i];
                        var to = values[j];

                        SearchStateOnAction(states, current, from, to);
                        if(Finished)
                        {
                            return true;
                        }
                    }
                }
            }
        }

        Log.Information("当前状态没有发现有效路径。返回上一步。。");
        return false;
    }


    
    public static void SearchStateOnAction(LinkedList<GameLogic> states, GameLogic current, Vector2Int from, Vector2Int to)
    {
        bool isVer = true;
        {
            if (current.CanMergeAction(from, to, isVer, out var offset))
            {
                // Log.Information($"CanMergeAction:{from},{to},Vertical");
                GameLogic next = new GameLogic(current.GameBoard.DeepClone());
                next.MergeAction(from,to,offset,Math.Abs(to.y - from.y),Tools.GetDir(from,to,isVer));
                // next.PrintState();
                states.AddLast(next);
                if (SearchState(states))
                {
                    return;
                }
                states.RemoveLast();
            }
        }

        {
            isVer = false;
            if (current.CanMergeAction(from, to, isVer, out var offset))
            {
                // Log.Information($"CanMergeAction:{from},{to},Horizontal");
                GameLogic next = new GameLogic(current.GameBoard.DeepClone());
                next.MergeAction(from,to,offset,Math.Abs(to.x - from.x),Tools.GetDir(from,to,isVer));
                states.AddLast(next);
                if (SearchState(states))
                {
                    return;
                }
                states.RemoveLast();
            }
        }
    }

    public static void PrintResults(LinkedList<GameLogic> states)
    {
        Log.Information("发现可解路径，移动过程如下：");
        foreach (var state in states)
        {
            state.PrintState();
        }
    }
}