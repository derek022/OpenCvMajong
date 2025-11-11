using OpenCvSharp;

namespace Mahjong.Recognition.FinalSolu;

public class MahjongTemplateMatcher
{
    public static List<MatchResult> FindAllUniqueMatches(
        string bigImagePath,
        Mat template,
        double minScale = 0.725,
        double maxScale = 0.716,
        double step = 0.05,
        double threshold = 0.72)
    {
    // 查找所有唯一匹配项
        using var bigImg = new Mat(bigImagePath);
        if (bigImg.Empty())
            throw new ArgumentException($"无法加载大图: {bigImagePath}");

        var results = new List<MatchResult>();

        for (double scale = minScale; scale <= maxScale; scale += step)
        {
            // 缩放模板
            int newW = (int)(template.Width * scale);
            int newH = (int)(template.Height * scale);
            if (newW <= 0 || newH <= 0) continue;

            using var resizedTemplate = new Mat();
            Cv2.Resize(template, resizedTemplate, new Size(newW, newH), 0, 0, InterpolationFlags.Area);

            // 执行模板匹配
            using var resultMat = new Mat();
            Cv2.MatchTemplate(bigImg, resizedTemplate, resultMat, TemplateMatchModes.CCoeffNormed);

            // 创建副本用于掩码
            using var resultCopy = resultMat.Clone();

            int i = 0;
            while (true)
            {
                i++;
                // 找最高分位置
                double minVal, maxVal;
                OpenCvSharp.Point minLoc, maxLoc;
                Cv2.MinMaxLoc(resultCopy, out minVal, out maxVal, out minLoc, out maxLoc);

                if (maxVal < threshold)
                {
                    break;
                }

                // 记录结果
                results.Add(new MatchResult
                {
                    X = maxLoc.X,
                    Y = maxLoc.Y,
                    Scale = scale,
                    Score = maxVal
                });

                // 创建掩码：覆盖匹配区域
                int maskSize = (int)(Math.Max(template.Width, template.Height) * scale * 0.7);
                int x1 = Math.Max(0, maxLoc.X - maskSize / 2);
                int y1 = Math.Max(0, maxLoc.Y - maskSize / 2);
                int x2 = Math.Min(resultCopy.Cols, maxLoc.X + maskSize / 2 + resizedTemplate.Width);
                int y2 = Math.Min(resultCopy.Rows, maxLoc.Y + maskSize / 2 + resizedTemplate.Height);

                // 将掩码区域置为 -1（无效值）
                var roi = new Mat(resultCopy, new Rect(x1, y1, x2 - x1, y2 - y1));
                roi.SetTo(new Scalar(-1));
                // Cv2.ImWrite($"roi_{i}.png", resultCopy);
            }
        }

        // 按得分降序排列
        results = results.OrderByDescending(r => r.Score).ToList();
        Console.WriteLine($"共找到 {results.Count} 个唯一匹配项。");
        return results;
    }

    // 可视化结果（保存为文件）
    public static void DrawMatches(string bigImagePath, List<MatchResult> matches, Mat template, string outputPath)
    {
        using var img = new Mat(bigImagePath);
        int idx = 1;

        foreach (var match in matches)
        {
            int w = (int)(template.Width * match.Scale);
            int h = (int)(template.Height * match.Scale);
            var pt1 = new OpenCvSharp.Point(match.X, match.Y);
            var pt2 = new OpenCvSharp.Point(match.X + w, match.Y + h);

            Cv2.Rectangle(img, pt1, pt2, Scalar.Red, 2);
            Cv2.PutText(img, $"{idx}: {match.Score:F3}", 
                new OpenCvSharp.Point(match.X + 5, match.Y + 20),
                HersheyFonts.HersheySimplex, 0.6, Scalar.Red, 2);
            idx++;
        }

        img.SaveImage(outputPath);
        Console.WriteLine($"结果已保存至: {outputPath}");
    }
}