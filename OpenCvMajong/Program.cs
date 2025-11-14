using System.Text.Json;
using Mahjong.Core;
using Mahjong.Core.Util;
using Mahjong.Recognition.FinalSolu;
using Mahjong.Resolution;
using Serilog;
using Serilog.Events;

namespace Mahjong;

public partial class Program
{
    
    static async Task Main(string[] args)
    {
        InitLogger();
        // Swipe(new Vector2Int(4,2),new Vector2Int(4,3));
        await RunAsync();
    }


    private static async Task RunAsync()
    {
        Log.Information("输入任意按键开始...");
        while (true)
        {
            Console.ReadKey();
            var ret = await ExecuteOnceAsync();
            if (!ret)
            {
                Log.Error("失败。");
                break;
            }
            Log.Information("输入任意按键，开启下一个 stage.");
        }
    }
    
    private static async Task<bool> ExecuteOnceAsync()
    {
        // 截图
        var screenFile = "screen.png";
        InputHelper.Screenshot(screenFile);
        await Task.Delay(500);
        // 图像识别
        var initBoard = Screen2DigitalData.Execute(screenFile, "Res/Prepared", Config.ScaleRange.X, Config.ScaleRange.Y);
        // 自动解析
        var results = AutoResolve.Init(initBoard);

        if (results.Count == 0)
        {
            Log.Error("No results found");
            return false;
        }

        var json = JsonSerializer.Serialize(results);
        await File.WriteAllTextAsync("temp.json", json);
        // 移除初始状态
        // results.RemoveFirst();
        // 根据结果，移动方块，滑动屏幕
        foreach (var step in results)
        {
            var action = step.GameBoard.CurrentAction;
            if(action is null)
                continue;

            var movePos = Action2MovePos(action);
            step.GameBoard.PrintState();
            Swipe(action.StartPos,movePos);
            await Task.Delay(TimeSpan.FromSeconds(1f));
        }

        return true;
    }

    private static Vector2Int Action2MovePos(MoveAction action)
    {
        var movePos = new Vector2Int(action.StartPos.x, action.StartPos.y);
        if (action.Direction == Direction.ToRight || action.Direction == Direction.ToLeft)
        {
            movePos.x = action.EndPos.x;
        }
        else if (action.Direction == Direction.ToDown || action.Direction == Direction.ToUp)
        {
            movePos.y = action.EndPos.y;
        }

        return movePos;
    }
    
    private static void InitLogger()
    {
        File.Delete("logs/log.txt");
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
    

}