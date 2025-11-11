using Mahjong.Core;
using Mahjong.Core.Util;
using Mahjong.Recognition.FinalSolu;
using Mahjong.Resolution;
using OpenCvSharp;
using Serilog;
using Serilog.Events;

class Program
{
    
    static async Task Main(string[] args)
    {
        InitLogger();
        TestMatcher();
    }

    private static void TestScreenPos2DigitalPos()
    {
        Screen2DigitalData.Execute("Res/Pics/Daily_Mahjong_Match.jpg", "Res/Prepared");
    }
    
    private static void TestMatcher()
    {
        string bigImagePath = "Res/Pics/Daily_Mahjong_Match.jpg";
        string resultPath = "matched_result.jpg";

        var croppedTemplate = Cv2.ImRead("Res/Prepared/ThreeBmb.png");
        //
        // double minScale = 0.725,
        // double maxScale = 0.716,
        // double step = 0.05,
        // double threshold = 0.72
        var matches = MahjongTemplateMatcher.FindAllUniqueMatches(
            bigImagePath: bigImagePath,
            template: croppedTemplate,
            minScale: 0.725,
            maxScale: 0.726,
            step: 0.05,
            threshold: 0.82
        );
        
        // 3. 可视化并保存
        if (matches.Any())
        {
            MahjongTemplateMatcher.DrawMatches(bigImagePath, matches, croppedTemplate, resultPath);
        }
    }

    private static void InitLogger()
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
            .WriteTo.Async(f => f.File("logs/log.txt"))
            .CreateLogger();
        AppDomain.CurrentDomain.UnhandledException += (s, e) =>
        {
            Log.Error(e.ExceptionObject as Exception, "Unhandled exception");
        };

    }
    private static void PrepareCards()
    {
        foreach (var file in Directory.GetFiles("Res/Cards/", "*.*", SearchOption.AllDirectories))
        {
            CropTool.Entry(file,"Res/"+Path.GetFileName(file),0);
        }
    }
    
    private static void Test()
    {
        GameBoard board = new GameBoard();
        board.SetBoardData(TestBoardData.Test1);
        GameLogic gameLogic = new GameLogic(board);
        var start = new Vector2Int(1, 2);
        var target = new Vector2Int(2, 1);
        bool isVerMove = true;
        
        if (gameLogic.CanMergeAction(start, target, isVerMove, out var offset,out var distance))
        {
            Log.Information("Merge action");
            gameLogic.MergeAction(start,target,offset,distance,Tools.GetDir(start,target,isVerMove));
            
            gameLogic.PrintState();
        }
    }

    
    private static async Task TestInputAsync()
    {
        Swipe(new Vector2Int(7, 7), new Vector2Int(7, 9));
    }


    private static void Swipe(Vector2Int start,Vector2Int move)
    {
        var offset = new Vector2Int(0, 5);
        start = (start + offset) * 100;
        move = (move + offset) * 100;
        InputHelper.SwipeScreen(start.x,start.y,move.x,move.y);
    }
    

}