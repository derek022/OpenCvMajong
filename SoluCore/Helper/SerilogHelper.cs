using Serilog;
using Serilog.Events;

namespace SoluCore.Helper;

public class SerilogHelper
{
    
    public static void InitLogger()
    {
        if(File.Exists("logs/log.txt"))
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