using OpenCvSharp;
using Serilog;
using Serilog.Core;

namespace Mahjong.Recognition.FinalSolu;

/*
    : 增大 step 值，或使用 多尺度粗搜索 + 单尺度精搜索
   
   思想: 先用一个较大的 step 进行一次粗略搜索，找到得分最高的几个 scale 值，然后在这些 scale 值附近用较小的 step 进行精细搜索。
   实现:
   第一轮: 使用 minScale, maxScale, largeStep (例如 step * 3) 找到 Top N 个候选 scale。
   第二轮: 对每个 Top N 的候选 scale，在其附近（例如 scale - largeStep/2 到 scale + largeStep/2）用原始的 step 进行精细搜索。
   
 */
public class MahjongTemplateV2Matcher
{
    protected static readonly ILogger Logger = Log.ForContext<CardRecognition>();

    // 粗搜索 + 精搜索 ---
    public static List<MatchResult> FindAllUniqueMatches(
        Mat bigImg,
        Mat template,
        double minScale = 0.71,
        double maxScale = 0.7281,
        double step = 0.002,
        double threshold = 0.9)
    {
        var results = new List<MatchResult>();
        var confirmedRegions = new HashSet<GridPoint>();
        int gridCellSize = 100;

        // 1. 粗搜索: 找到最佳的几个缩放比例
        double largeStep = step * 3; // 例如，将步长增大3倍
        int numScales = (int)((maxScale - minScale) / largeStep) + 1;
        var topScales = new List<(double scale, double maxScore)>();

        for (int i = 0; i < numScales; i++)
        {
            double scale = minScale + i * largeStep;
            if (scale > maxScale) break;

            int newW = (int)(template.Width * scale);
            int newH = (int)(template.Height * scale);
            if (newW <= 0 || newH <= 0) continue;

            using var resizedTemplate = new Mat();
            Cv2.Resize(template, resizedTemplate, new Size(newW, newH), 0, 0, InterpolationFlags.Area);

            using var resultMat = new Mat();
            Cv2.MatchTemplate(bigImg, resizedTemplate, resultMat, TemplateMatchModes.CCoeffNormed);

            double minVal, maxVal;
            Point minLoc, maxLoc;
            Cv2.MinMaxLoc(resultMat, out minVal, out maxVal, out minLoc, out maxLoc);

            topScales.Add((scale, maxVal));
        }

        // 选择得分最高的几个 scale (例如前3个)
        var bestScales = topScales.OrderByDescending(x => x.maxScore).Take(3).Select(x => x.scale).ToList();

        // 2. 精搜索: 只在最佳 scale 附近进行详细搜索
        foreach (var bestScale in bestScales)
        {
            // 定义精细搜索的范围
            double fineMinScale = Math.Max(minScale, bestScale - largeStep / 2.0);
            double fineMaxScale = Math.Min(maxScale, bestScale + largeStep / 2.0);

            // 重新执行原始的精细搜索逻辑，但范围缩小
            // 为了代码复用，可以将核心搜索逻辑提取为一个私有辅助方法
            PerformFineSearch(bigImg, template, fineMinScale, fineMaxScale, step, threshold, gridCellSize, results,
                confirmedRegions);
        }

        results = results.OrderByDescending(r => r.Score).ToList();
        Logger.Information($"共找到 {results.Count} 个唯一匹配项。");
        return results;
    }

    // 提取核心搜索逻辑到辅助方法
    private static void PerformFineSearch(
        Mat bigImg, Mat template, double minScale, double maxScale, double step, double threshold,
        int gridCellSize, List<MatchResult> results, HashSet<GridPoint> confirmedRegions)
    {
        for (double scale = minScale; scale <= maxScale; scale += step)
        {
            int newW = (int)(template.Width * scale);
            int newH = (int)(template.Height * scale);
            if (newW <= 0 || newH <= 0) continue;

            using var resizedTemplate = new Mat();
            Cv2.Resize(template, resizedTemplate, new Size(newW, newH), 0, 0, InterpolationFlags.Area);

            using var resultMat = new Mat();
            Cv2.MatchTemplate(bigImg, resizedTemplate, resultMat, TemplateMatchModes.CCoeffNormed);

            using var resultCopy = resultMat.Clone();

            int i = 0;
            while (true)
            {
                i++;
                double minVal, maxVal;
                Point minLoc, maxLoc;
                Cv2.MinMaxLoc(resultCopy, out minVal, out maxVal, out minLoc, out maxLoc);

                if (maxVal < threshold)
                {
                    break;
                }

                var gridLoc = new GridPoint(maxLoc.X / gridCellSize, maxLoc.Y / gridCellSize);

                bool isNearbyConfirmed = false;
                for (int dx = -1; dx <= 1; dx++)
                {
                    for (int dy = -1; dy <= 1; dy++)
                    {
                        var neighborGrid = new GridPoint(gridLoc.X + dx, gridLoc.Y + dy);
                        if (confirmedRegions.Contains(neighborGrid))
                        {
                            isNearbyConfirmed = true;
                            goto BreakLoops;
                        }
                    }
                }

                BreakLoops:

                if (!isNearbyConfirmed)
                {
                    results.Add(new MatchResult
                    {
                        X = maxLoc.X,
                        Y = maxLoc.Y,
                        Scale = scale,
                        Score = maxVal
                    });
                    confirmedRegions.Add(gridLoc);
                }

                int maskSize = (int)(Math.Max(template.Width, template.Height) * scale * 0.7);
                int x1 = Math.Max(0, maxLoc.X - maskSize / 2);
                int y1 = Math.Max(0, maxLoc.Y - maskSize / 2);
                int x2 = Math.Min(resultCopy.Cols, maxLoc.X + maskSize / 2 + resizedTemplate.Width);
                int y2 = Math.Min(resultCopy.Rows, maxLoc.Y + maskSize / 2 + resizedTemplate.Height);

                var roi = new Mat(resultCopy, new Rect(x1, y1, x2 - x1, y2 - y1));
                roi.SetTo(new Scalar(-1));
            }
        }
    }
}