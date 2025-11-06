using Mahjong.Core;
using Mahjong.Core.Util;
using Serilog;

namespace Mahjong.Resolution;

public class AutoResolve
{
    public static void Init(Cards[,] initBoard)
    {
        var board = new GameBoard();
        board.SetBoardData(initBoard);
        
        GameLogic logic = new GameLogic(board);
        
        LinkedList<GameLogic> states = new();
        states.AddLast(logic);
        SearchState(states);
    }
    
    public static void SearchState(LinkedList<GameLogic> states)
    {
        var current = states.Last();
        if (current.IsFinalState())
        {
            PrintResult(states);
            return;
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
                        
                        SearchStateOnAction(states,current,from,to);
                    }
                }
            }
        }
        
        
    }

    public static void SearchStateOnAction(LinkedList<GameLogic> states, GameLogic current, Vector2Int from, Vector2Int to)
    {
        {
            if (current.CanMergeAction(from, to, true, out var offset))
            {
                Log.Information($"CanMergeAction:{from},{to},true");
                GameLogic next = new GameLogic(current.GameBoard.DeepClone());
                next.MergeAction(from,to,offset,Math.Abs(to.y - from.y));
                next.PrintState();
                states.AddLast(next);
                SearchState(states);
                states.RemoveLast();
            }
        }

        {
            if (current.CanMergeAction(from, to, false, out var offset))
            {
                Log.Information($"CanMergeAction:{from},{to},false");
                GameLogic next = new GameLogic(current.GameBoard.DeepClone());
                next.MergeAction(from,to,offset,Math.Abs(to.y - from.y));
                next.PrintState();
                states.AddLast(next);
                SearchState(states);
                states.RemoveLast();
            }
        }
    }

    public static void PrintResult(LinkedList<GameLogic> states)
    {
        Console.WriteLine("发现可解路径，移动过程如下：");
        foreach (var state in states)
        {
            state.PrintState();
        }
    }
}