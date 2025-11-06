using System.Text.Json;
using OpenCvSharp;

namespace Mahjong.Recognition.Gpt;

public class Match
{
    public static void Entry()
    {
        string bigPath = "Pics/Daily_Mahjong_Match.jpg";
        string smallPath = "Cards/43.png";

        using var big = Cv2.ImRead(bigPath, ImreadModes.Color);
        using var bigGray = new Mat();
        Cv2.CvtColor(big, bigGray, ColorConversionCodes.BGR2GRAY);

        // foreach (var smallFile in Directory.GetFiles(smallPath,"*.png",SearchOption.AllDirectories))
        {
            using var small = Cv2.ImRead(smallPath, ImreadModes.Color);
            using var smallGray = new Mat();
            Cv2.CvtColor(small, smallGray, ColorConversionCodes.BGR2GRAY);

            double bestScore = 0;
            double bestScale = 1.0;
            Rect bestRect = new Rect();

            // 多尺度匹配
            // for (double scale = 0.5; scale <= 1.5; scale += 0.05)
            var scale = 0.7;
            {
                int newW = (int)(smallGray.Width * scale);
                int newH = (int)(smallGray.Height * scale);
                if (newW < 5 || newH < 5 || newW > bigGray.Width || newH > bigGray.Height)
                    return;

                using var resized = smallGray.Resize(new Size(newW, newH));

                using var result = bigGray.MatchTemplate(resized, TemplateMatchModes.CCoeffNormed);

                
                using var mask = new Mat();
                double threshold = 0.8;
                Cv2.Threshold(result, mask, threshold, 1.0, ThresholdTypes.Tozero);
                var nonZero = mask.FindNonZero(); // ✅ 正确写法

                if (nonZero.Empty()) return;
                
                // var points = new List<Point>();
                List<Rect> rects = new List<Rect>();
                for (int i = 0; i < nonZero.Rows; i++)
                {
                    var xy = nonZero.At<Point>(i);
                    // points.Add(xy);
                    rects.Add( new Rect(xy.X, xy.Y, resized.Width, resized.Height));
                    Console.WriteLine(xy);
                }
                foreach (var rect in rects)
                {
                    Cv2.Rectangle(big, rect, new Scalar(0, 0, 255), 2);
                }
                
                Console.WriteLine("scale:"+scale+" count:"+rects.Count);
                Cv2.ImShow("big",big);
                Cv2.MinMaxLoc(result, out _, out double maxVal, out _, out Point maxLoc);

                if (maxVal > bestScore)
                {
                    bestScore = maxVal;
                    bestScale = scale;
                    bestRect = new Rect(maxLoc.X, maxLoc.Y, resized.Width, resized.Height);
                    Cv2.ImShow("aled",result);
                }
            }

        }

        Cv2.WaitKey();

    }

    class MatchResult
    {
        public string Name { get; set; }
        public double Scale { get; set; }
        public double Score { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }

    public static void Entry2()
    {
        string bigPath = "Pics/Daily_Mahjong_Match.jpg";
        string smallDir = "Cards";
        string outputJson = "result.json";
        
        using var bigColor = Cv2.ImRead(bigPath, ImreadModes.Color);
        if (bigColor.Empty())
        {
            Console.WriteLine("大图加载失败");
            return;
        }
        
        // 转灰度可以提升速度 & 稳定性
        using var bigGray = new Mat();
        Cv2.CvtColor(bigColor, bigGray, ColorConversionCodes.BGR2GRAY);
        
        var results = new List<MatchResult>();

        var random = new Random();
        foreach (var file in Directory.GetFiles(smallDir, "*.*", SearchOption.AllDirectories))
        {
            if (!file.EndsWith(".png") && !file.EndsWith(".jpg")) continue;
            using var smallColor = Cv2.ImRead(file, ImreadModes.Color);
            if (smallColor.Empty()) continue;
        
            using var smallGray = new Mat();
            Cv2.CvtColor(smallColor, smallGray, ColorConversionCodes.BGR2GRAY);
        
            Console.WriteLine($"🔍 搜索 {Path.GetFileName(file)} ...");
        
            for (double scale = 0.7; scale <= 0.8; scale += 0.05)
            {
                // var color = random.Next(0, 255);
                int w = (int)(smallGray.Width * scale);
                int h = (int)(smallGray.Height * scale);
                if (w < 5 || h < 5 || w > bigGray.Width || h > bigGray.Height) continue;
        
                using var resized = smallGray.Resize(new Size(w, h));
                using var result = bigGray.MatchTemplate(resized, TemplateMatchModes.CCoeffNormed);
        
                double threshold = 0.8;
                // 阈值过滤
                using var mask = new Mat();
                Cv2.Threshold(result, mask, threshold, 1.0, ThresholdTypes.Tozero);
        
                // 找出所有非零位置
                using var nonZero = new Mat();
                Cv2.FindNonZero(mask, nonZero); // ✅ 正确写法

                if (nonZero.Empty()) continue;
        
                var points = new List<Point>();
                for (int i = 0; i < nonZero.Rows; i++)
                {
                    var xy = nonZero.At<Point>(i);
                    points.Add(xy);
                }
                
                foreach (var p in points)
                {
                    double score = result.Get<float>(p.Y, p.X);
                    var rect = new Rect(p.X, p.Y, w, h);
        
                    // 过滤太接近的点（避免重叠）
                    bool overlap = false;
                    foreach (var r in results)
                    {
                        if (r.Name == Path.GetFileName(file) &&
                            Math.Abs(r.X - rect.X) < w / 4 &&
                            Math.Abs(r.Y - rect.Y) < h / 4)
                        {
                            overlap = true;
                            break;
                        }
                    }
        
                    if (!overlap)
                    {
                        results.Add(new MatchResult
                        {
                            Name = Path.GetFileName(file),
                            Scale = scale,
                            Score = score,
                            X = rect.X,
                            Y = rect.Y,
                            Width = rect.Width,
                            Height = rect.Height
                        });
        
                        Cv2.Rectangle(bigColor, rect, new Scalar(0, 0, 255), 2);
                    }
                }
            }
        }

        Cv2.ImWrite("matched_multi.png", bigColor);
        File.WriteAllText("result.json",
            JsonSerializer.Serialize(results, new JsonSerializerOptions { WriteIndented = true }));
        
        Console.WriteLine($"✅ 匹配完成，共 {results.Count} 个匹配点。结果已输出到 result.json 和 matched_multi.png。");

    }
    
    
    public static List<Rect> FindTemplateMatchesMultiScale(Mat source, Mat template,
        double scaleMin = 0.5, double scaleMax = 1.0, double scaleStep = 0.05, double threshold = 0.8)
    {
        var allMatches = new List<Rect>();

        Mat sourceGray = new Mat();
        Mat templateGray = new Mat();

        if (source.Channels() == 3)
            Cv2.CvtColor(source, sourceGray, ColorConversionCodes.BGR2GRAY);
        else
            sourceGray = source;

        if (template.Channels() == 3)
            Cv2.CvtColor(template, templateGray, ColorConversionCodes.BGR2GRAY);
        else
            templateGray = template;

        // 多尺度搜索
        for (double scale = scaleMin; scale <= scaleMax; scale += scaleStep)
        {
            Console.WriteLine("current scale:" + scale);
            // 缩放模板
            Mat resizedTemplate = new Mat();
            Cv2.Resize(templateGray, resizedTemplate,
                new Size(templateGray.Width * scale, templateGray.Height * scale));

            // 如果缩放后的模板比原图大，跳过
            if (resizedTemplate.Width > sourceGray.Width || resizedTemplate.Height > sourceGray.Height)
                continue;

            Mat result = new Mat();
            Cv2.MatchTemplate(sourceGray, resizedTemplate, result, TemplateMatchModes.CCoeffNormed);

            // 找到匹配位置
            result.MinMaxLoc(out double minVal, out double maxVal, out Point minLoc, out Point maxLoc);

            if (maxVal >= threshold)
            {
                Rect matchRect = new Rect(maxLoc, resizedTemplate.Size());
                allMatches.Add(matchRect);
                Console.WriteLine($"找到匹配位置: X={matchRect.X}, Y={matchRect.Y}, Width={matchRect.Width}, Height={matchRect.Height}");
            }
        }

        return allMatches;
    }

    public static List<Rect> FindTemplateMatchesWithNMS(Mat source, Mat template,
        double threshold = 0.8, double nmsThreshold = 0.3)
    {
        var matches = new List<Rect>();
        var scores = new List<double>();

        Mat sourceGray = new Mat();
        Mat templateGray = new Mat();

        if (source.Channels() == 3)
            Cv2.CvtColor(source, sourceGray, ColorConversionCodes.BGR2GRAY);
        else
            sourceGray = source;

        if (template.Channels() == 3)
            Cv2.CvtColor(template, templateGray, ColorConversionCodes.BGR2GRAY);
        else
            templateGray = template;

        Mat result = new Mat();
        Cv2.MatchTemplate(sourceGray, templateGray, result, TemplateMatchModes.CCoeffNormed);

        // 收集所有超过阈值的匹配
        for (int y = 0; y < result.Height; y++)
        {
            for (int x = 0; x < result.Width; x++)
            {
                double score = result.At<float>(y, x);
                if (score >= threshold)
                {
                    matches.Add(new Rect(x, y, template.Width, template.Height));
                    scores.Add(score);
                }
            }
        }

        // 应用非极大值抑制
        return ApplyNMS(matches, scores, nmsThreshold);
    }

    private static List<Rect> ApplyNMS(List<Rect> boxes, List<double> scores, double threshold)
    {
        if (boxes.Count == 0) return new List<Rect>();

        // 根据分数排序
        var indices = new List<int>();
        for (int i = 0; i < boxes.Count; i++) indices.Add(i);

        indices.Sort((a, b) => scores[b].CompareTo(scores[a]));

        var picked = new List<int>();

        while (indices.Count > 0)
        {
            int current = indices[0];
            picked.Add(current);

            for (int i = indices.Count - 1; i > 0; i--)
            {
                int idx = indices[i];
                double iou = CalculateIOU(boxes[current], boxes[idx]);

                if (iou > threshold)
                {
                    indices.RemoveAt(i);
                }
            }

            indices.RemoveAt(0);
        }

        var result = new List<Rect>();
        foreach (int index in picked)
        {
            result.Add(boxes[index]);
        }

        return result;
    }

    private static double CalculateIOU(Rect rect1, Rect rect2)
    {
        Rect intersection = rect1 & rect2;
        double intersectionArea = intersection.Width * intersection.Height;
        double unionArea = (rect1.Width * rect1.Height) + (rect2.Width * rect2.Height) - intersectionArea;

        return intersectionArea / unionArea;
    }
}