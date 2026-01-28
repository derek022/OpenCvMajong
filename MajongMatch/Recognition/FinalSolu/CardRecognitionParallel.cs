using System.Collections.Concurrent;
using System.Diagnostics;
using Mahjong.Core;
using Mahjong.Core.Util;
using OpenCvSharp;
using Serilog;

namespace Mahjong.Recognition.FinalSolu;


public class CardRecognitionParallel
{
    protected static readonly ILogger Logger = Log.ForContext<CardRecognitionParallel>();
    
    /// <summary>
    /// 卡片识别主函数
    /// </summary>
    /// <param name="screenShot"></param>
    /// <param name="templateDir"></param>
    /// <param name="minScale"></param>
    /// <param name="maxScale"></param>
    /// <returns></returns>
    public static Cards[,] Execute(string screenShot, string templateDir, float minScale,float maxScale)
    {
        var swTotal = Stopwatch.StartNew(); // 总时间计时

        var initBoard = new Cards[12, 10];
        var allResults = new ConcurrentBag<(Cards cardType, Vector2Int pos)>();

        // 1. === 串行预加载所有模板到内存 ===
        
        Logger.Information("开始预加载模板...");
        var swPreload = Stopwatch.StartNew();
        var templateFileNames = Directory.GetFiles(templateDir, "*.png");
        var inMemoryTemplates = new List<(Cards cardType, Mat template)>();
        // 加载大图
        using var screenShotMat = new Mat(screenShot);
        foreach (var filePath in templateFileNames)
        {
            try
            {
                var cardName = Path.GetFileNameWithoutExtension(filePath);
                var cardEnum = Enum.Parse<Cards>(cardName);
                var template = new Mat(filePath); // 加载到内存
                inMemoryTemplates.Add((cardEnum, template));
            }
            catch (Exception ex)
            {
                Logger.Error(ex, $"加载模板文件失败: {filePath}");
                // 可以选择跳过或抛出异常
            }
        }
        swPreload.Stop();
        Logger.Information($"预加载 {inMemoryTemplates.Count} 个模板耗时: {swPreload.ElapsedMilliseconds} ms");

        // 2. === 并行处理内存中的模板 ===
        Logger.Information("开始并行识别...");
        var swParallel = Stopwatch.StartNew();
        var parallelOptions = new ParallelOptions
        {
            MaxDegreeOfParallelism = Environment.ProcessorCount // 或者指定一个数字，如 4
        };
        Parallel.ForEach(inMemoryTemplates, parallelOptions,item =>
        {
            var (cardEnum, template) = item;
            var swTask = Stopwatch.StartNew(); // 记录单个任务耗时

            Logger.Debug($"[Thread {Thread.CurrentThread.ManagedThreadId}] 开始查找：{cardEnum}");

            try
            {
                var results = MahjongTemplateMatcher.FindAllUniqueMatches(screenShotMat, template, minScale, maxScale);

                swTask.Stop();
                Logger.Debug($"[Thread {Thread.CurrentThread.ManagedThreadId}] 查找 {cardEnum} 完成，耗时 {swTask.ElapsedMilliseconds} ms, 找到 {results.Count} 个匹配");
                
                foreach (var pos in results)
                {
                    var realPos = new Vector2Int(pos.X / 100, (pos.Y - 500) / 100);
                    allResults.Add((cardEnum, realPos)); // 添加到线程安全集合
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, $"处理模板 {cardEnum} 时出错");
                swTask.Stop(); // 确保即使出错也停止计时
            }
        });
        swParallel.Stop();
        Logger.Information($"并行识别耗时: {swParallel.ElapsedMilliseconds} ms");

        // 3. === 合并结果到 initBoard ===
        Logger.Information("开始合并结果...");
        var swMerge = Stopwatch.StartNew();
        var boardLock = new object();
        foreach (var (cardType, pos) in allResults)
        {
            lock (boardLock) // 保护对 initBoard 的写入
            {
                if (pos.y >= 0 && pos.y < initBoard.GetLength(0) && pos.x >= 0 && pos.x < initBoard.GetLength(1))
                {
                    if (initBoard[pos.y, pos.x] == Cards.Zero || initBoard[pos.y, pos.x] == cardType)
                    {
                        initBoard[pos.y, pos.x] = cardType;
                    }
                    else
                    {
                        Logger.Error($"坐标 ({pos.y}, {pos.x}) 出现冲突，已有值 {initBoard[pos.y, pos.x].ToString()}，新值 {cardType.ToString()}");
                    }
                }
                else
                {
                    Logger.Error($"转换后的坐标 ({pos.y}, {pos.x}) 超出棋盘范围 [0,0] 到 [{initBoard.GetLength(0) - 1}, {initBoard.GetLength(1) - 1}]");
                }
            }
        }
        swMerge.Stop();
        Logger.Information($"合并结果耗时: {swMerge.ElapsedMilliseconds} ms");

        // 4. 检查未填充的位置（可选）
        for (int i = 0; i < initBoard.GetLength(0); i++)
        {
            for (int j = 0; j < initBoard.GetLength(1); j++)
            {
                if (initBoard[i, j] == Cards.Zero)
                {
                    Logger.Debug($"坐标 ({i}, {j}) 未识别到牌");
                }
            }
        }

        swTotal.Stop();
        Logger.Information($"总耗时: {swTotal.ElapsedMilliseconds} ms");

        // 5. === 释放预加载的 Mat 资源 ===
        foreach (var (_, template) in inMemoryTemplates)
        {
            template?.Dispose();
        }

        return initBoard;
    }
}