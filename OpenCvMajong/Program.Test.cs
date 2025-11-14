using Mahjong.Core;
using Mahjong.Core.Util;
using Mahjong.Recognition.FinalSolu;
using Mahjong.Resolution;
using OpenCvSharp;
using Serilog;

namespace Mahjong;

public partial class Program
{
    
    private static void TestMatcher()
    {
        string bigImagePath = "Res/Pics/Daily_Mahjong_Match.jpg";
        string resultPath = "matched_result.jpg";

        var croppedTemplate = Cv2.ImRead("Res/Prepared/FourDots.png");
        
        // double minScale = 0.725,
        // double maxScale = 0.716,
        // double step = 0.05,
        // double threshold = 0.72
        var matches = MahjongTemplateMatcher.FindAllUniqueMatches(
            bigImagePath: bigImagePath,
            template: croppedTemplate
        );
        
        // 3. 可视化并保存
        if (matches.Any())
        {
            MahjongTemplateMatcher.DrawMatches(bigImagePath, matches, croppedTemplate, resultPath);
        }
    }
    
    
    private async static Task TestScreenPos2DigitalPos()
    {
        var screenFile = "screen.png";
        InputHelper.Screenshot(screenFile);
        await Task.Delay(500);
        
        var initBaord = Screen2DigitalData.Execute(screenFile, "Res/Prepared", Config.ScaleRange.X,
            Config.ScaleRange.Y);

        GameBoard board = new GameBoard();
        board.SetBoardData(initBaord);
        board.PrintState();
        var steps = AutoResolve.Init(initBaord);

        foreach (var step in steps)
        {
            var action = step.GameBoard.CurrentAction;
            if(action is null)
                continue;
            
            var movePos = Action2MovePos(action);
            
            step.GameBoard.PrintState();
            Swipe(action.StartPos,movePos);
            await Task.Delay(TimeSpan.FromSeconds(2f));
        }
    }
    

    
    private static void PrepareCards()
    {
        foreach (var file in Directory.GetFiles("Res/Cards/", "*.png", SearchOption.AllDirectories))
        {
            CropTool.Entry(file,"Res/Prepared2/"+Path.GetFileName(file),-5);
        }
    }
    
    private static void Test()
    {
        GameBoard board = new GameBoard();
        board.SetBoardData(SampleBoards.TestData.Test1);
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