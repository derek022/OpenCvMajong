using Mahjong.Core;
using Mahjong.Resolution;
using Serilog;
using Serilog.Events;

class Program
{
    static void Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
#if DEBUG
            .MinimumLevel.Debug()
#else
                .MinimumLevel.Information()
#endif
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .Enrich.FromLogContext()
            .WriteTo.Async(c => c.Console())
            .CreateLogger();
        AppDomain.CurrentDomain.UnhandledException += (s, e) =>
        {
            Log.Error(e.ExceptionObject as Exception, "Unhandled exception");
        };

        AutoResolve.Init(SampleBoards.EaseBoard1);
        // Test();
    }

    private static void Test()
    {
        GameBoard board = new GameBoard();
        board.SetBoardData(TestBoardData.Test1);
        GameLogic gameLogic = new GameLogic(board);
        var start = new Vector2Int(2, 2);
        var target = new Vector2Int(2, 1);
        bool isVerMove = true;

        int GetDis()
        {
            if (isVerMove)
            {
                return Math.Abs(target.y - start.y);
            }
            return Math.Abs(start.x - target.x);
        }
        if (gameLogic.CanMergeAction(start, target, isVerMove, out var offset))
        {
            Log.Information("Merge action");
            gameLogic.MergeAction(start,target,offset,GetDis());
            
            gameLogic.PrintState();
        }
        
        
    }

}