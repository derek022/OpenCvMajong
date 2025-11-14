using Serilog;
using Serilog.Events;

namespace Mahjong;

public partial class Program
{
    
    static async Task Main(string[] args)
    {
        InitLogger();
        await ExecuteAsync();
    }


    private static async Task ExecuteAsync()
    {
        // 截图
        
        // 图像识别
        
        // 自动解析
        
        // 根据结果，移动方块，滑动屏幕
        
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
    

}