using System.Diagnostics;
using Mahjong.Core;

namespace Mahjong.Resolution;

public class InputHelper
{
    private static void RunSheel(string cmd)
    {
        ProcessStartInfo startInfo = new ProcessStartInfo
        {
            FileName = "adb", // 要执行的命令
            Arguments = cmd, // 命令的参数
            UseShellExecute = false, // 不使用操作系统shell执行
            RedirectStandardOutput = true, // 重定向标准输出
            RedirectStandardError = true, // 重定向标准错误
            CreateNoWindow = true, // 不创建窗口
        };
        
        using (Process process = Process.Start(startInfo))
        {
            // 捕获输出
            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();

            // 等待进程退出
            process.WaitForExit();

            // 可以在这里处理 output 和 error
            Console.WriteLine("输出:\n" + output);
            if (!string.IsNullOrEmpty(error))
            {
                Console.WriteLine("错误:\n" + error);
            }
        }
    }

    public static void ClickScreen(int x,int y)
    {
        RunSheel($"shell input tap {x} {y} ");
    }

    public static void SwipeScreen(int x, int y, int z, int w)
    {
        RunSheel($"shell input swipe {x} {y} {z} {w}");
    }

    public static void Screenshot(string fileName)
    {
        var psi = new ProcessStartInfo
        {
            FileName = "bash",
            Arguments = $"-c \"adb exec-out screencap -p > '{fileName}'\"",
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardError = true
        };

        using var process = Process.Start(psi);
        process.WaitForExit();

        if (process.ExitCode != 0)
        {
            var error = process.StandardError.ReadToEnd();
            throw new InvalidOperationException($"ADB screenshot failed: {error}");
        }

        Console.WriteLine($"Screenshot saved to {Path.GetFullPath(fileName)}");

    }
}