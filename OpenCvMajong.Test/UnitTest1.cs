using System.Text;
using Mahjong.Core;
using Mahjong.Core.Util;
using Mahjong.Recognition.FinalSolu;
using Mahjong.Resolution;
using Mahjong.Resolution.SearchState;
using OpenCvSharp;
using Serilog;
using Xunit.Abstractions;

namespace OpenCvMajong.Test;

public class UnitTest1
{
    private readonly ITestOutputHelper _testOutputHelper;

    public UnitTest1(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public void Test1()
    {
        _testOutputHelper.WriteLine("Test1");  
    }
    
    /// <summary>
    /// 测试单个模板图片识别
    /// </summary>
    [Fact]
    private static void TestSingleMatcher()
    {
        string bigImagePath = "screen_fail2.png";
        string resultPath = "matched_result.jpg";

        var croppedTemplate = Cv2.ImRead("Res/Prepared/NineDots.png");
        
        // double minScale = 0.725,
        // double maxScale = 0.716,
        // double step = 0.05,
        // double threshold = 0.72
        var bigMat = new Mat(bigImagePath);
        var matches = MahjongTemplateMatcher.FindAllUniqueMatches(
            bigImg:bigMat,
            template: croppedTemplate
        );
        
        // 3. 可视化并保存
        if (matches.Any())
        {
            MahjongTemplateMatcher.DrawMatches(bigImagePath, matches, croppedTemplate, resultPath);
        }
    }

    [Fact]
    private async static Task TestScreenPos2DigitalPos()
    {
        var screenFile = "screen.png";
        // InputHelper.Screenshot(screenFile);
        // await Task.Delay(500);

        var initBaord = CardRecognitionParallel.Execute(screenFile, "Res/Prepared", Config.ScaleRange.X,
            Config.ScaleRange.Y);

        GameBoard board = new GameBoard();
        board.SetBoardData(initBaord);
        board.PrintState();
        var steps = await AutoResolve.InitAsync<SearchStateVRecursion>(initBaord);
        
        if (steps == null)
            return;
        Log.Debug("发现了解题过程。。。。");
        foreach (var step in steps)
        {
            var action = step.GameBoard.CurrentAction;
            if (action is null)
                continue;
        
            // var movePos = Action2MovePos(action);
        
            step.PrintState();
            // Swipe(action.StartPos,movePos);
            // await Task.Delay(TimeSpan.FromSeconds(2f));
        }
    }

    
    private static void PrepareCards()
    {
        foreach (var file in Directory.GetFiles("Res/Cards/", "*.png", SearchOption.AllDirectories))
        {
            CropTool.Entry(file,"Res/Prepared2/"+Path.GetFileName(file),-5);
        }
    }
    
    /// <summary>
    /// 测试单个位置消除
    /// </summary>
    [Fact]
    private static void TestSingleMove()
    {
        GameBoard board = new GameBoard();
        board.SetBoardData(SampleBoards.TestData.Test1);
        GameLogic gameLogic = new GameLogic(board);
        var start = new Vector2Int(1, 2);
        var target = new Vector2Int(2, 1);
        bool isVerMove = true;
        
        if (gameLogic.CanMergeAction(start, target, isVerMove, out var offset,out var distance))
        {
            // Log.Information("Merge action");
            gameLogic.MergeAction(start,target,offset,distance,Tools.GetDir(start,target,isVerMove));
            
            gameLogic.PrintState();
        }
    }

    /// <summary>
    /// 测试结局棋盘
    /// </summary>
    [Fact]
    private static async Task TestResolve()
    {
        try
        {
            var steps = await AutoResolve.InitAsync<SearchStateVRecursion>(SampleBoards.ExpertBoard4);
            
            foreach (var step in steps)
            {
                step.PrintState();
            }
        }
        catch (Exception e)
        {
            throw; 
        }
    }

    
    private static void Print2csData(GameBoard board)
    {
        for (int i = 1; i < board.Height - 1; i++)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append($"{{");
            for (int j = 1; j < board.Width -1; j++)
            {
                builder.Append($"Cards.{board.GetCard(j, i)},");
            }
            builder.Append("},");
            Log.Information(builder.ToString());
        }
    }
    
    /// <summary>
    /// 输入测试
    /// </summary>
    private static async Task TestInputAsync()
    {
        Swipe(new Vector2Int(7, 7), new Vector2Int(7, 9));
        await Task.Delay(TimeSpan.FromSeconds(1));
    }
    

    private static void Swipe(Vector2Int start,Vector2Int move)
    {
        Log.Information($"Swipe:{start}-{move}");
        var offset = new Vector2Int(0, 5);
        start = (start + offset) * 100;
        move = (move + offset) * 100;
        InputHelper.SwipeScreen(start.x,start.y,move.x,move.y);
    }


}