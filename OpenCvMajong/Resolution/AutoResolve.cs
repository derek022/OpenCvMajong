using OpenCvMajong.Core;

namespace OpenCvMajong.Resolution;

public class AutoResolve
{
    public static void Init(Cards[,] initBoard)
    {
        var board = new GameBoard();
        board.InitBoard(initBoard);
        GameLogic logic;

        logic = new GameLogic();
        logic.SetBoard(board);
        
        Queue<GameLogic> states = new();
        states.Enqueue(logic);
        SearchState(states);
    }
    
    public static void SearchState(Queue<GameLogic> states)
    {
        var current = states.Peek();
        if (current.IsFinalState())
        {
            PrintResult(states);
            return;
        }

        foreach (var pair in current.GameBoard.CardPositions)
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

    public static void SearchStateOnAction(Queue<GameLogic> states, GameLogic current, CardPos from, CardPos to)
    {
        {
            if (current.CanMergeAction(from, to, true, out var offset))
            {
                GameLogic next = new GameLogic();
                next.SetBoard(current.GameBoard);
                next.MergeAction(from,to,offset,Math.Abs(to.Y - from.Y));
                states.Enqueue(next);
                SearchState(states);
                states.Dequeue();
            }
        }

        {
            if (current.CanMergeAction(from, to, false, out var offset))
            {
                GameLogic next = new GameLogic();
                next.SetBoard(current.GameBoard);
                next.MergeAction(from,to,offset,Math.Abs(to.Y - from.Y));
                states.Enqueue(next);
                SearchState(states);
                states.Dequeue();
            }
        }
    }

    public static void PrintResult(Queue<GameLogic> states)
    {
        Console.WriteLine("发现可解路径，移动过程如下：");
        foreach (var state in states)
        {
            state.PrintState();
        }
    }
}