using Mahjong.Core;
using Mahjong.Core.Util;
using Mahjong.Resolution.SearchState;
using Serilog;

namespace Mahjong.Resolution;

public class AutoResolve
{
    public static async Task<LinkedList<GameLogic>> InitAsync<T>(Cards[,] initBoard) where T : ISearchLogic ,new()
    {
        var board = new GameBoard();
        board.SetBoardData(initBoard);
        Log.Information("初始状态：");
        board.PrintState();
        GameLogic logic = new GameLogic(board);
        
        var search = new T();
        search.Initialize(logic);

        var result = await search.SearchState();
        PrintResults(result);
        return result;
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