using OpenCvSharp;
using Serilog;

namespace Mahjong.Recognition.FinalSolu;

public class CropTool
{
    protected static readonly ILogger Logger = Log.ForContext<CropTool>();
    
    /// <summary>
    /// 模板图片裁剪
    /// </summary>
    /// <param name="src"></param>
    /// <param name="dst"></param>
    /// <param name="margin"></param>
    public static void Entry(string src,string dst,int margin)
    {
        var res = AutoCropTemplate(src, margin);
        Cv2.ImWrite(dst, res);
    }

    private static Mat AutoCropTemplate(string filepath, int margin = 5)
    {
        Logger.Information("CropTool:" + filepath);
        using var img = Cv2.ImRead(filepath);
        using var grey = new Mat();
        using var binary = new Mat();
        Cv2.CvtColor(img, grey, ColorConversionCodes.BGR2GRAY);

        Cv2.Threshold(grey, binary, 200, 255, ThresholdTypes.Binary);

        var contours = Cv2.FindContoursAsArray(binary, RetrievalModes.External, ContourApproximationModes.ApproxSimple);

        if (contours.Length == 0)
        {
            Logger.Information("未找到任何轮廓，无法裁剪。");
            return img;
        }
        

        var largestContour = contours.OrderByDescending(c => Cv2.ContourArea(c)).First();
        var rect = Cv2.BoundingRect(largestContour);
        // 添加边距
        rect.X = Math.Max(0, rect.X - margin);
        rect.Y = Math.Max(0, rect.Y - margin);
        rect.Width = Math.Min(img.Width - rect.X, rect.Width + 2 * margin);
        rect.Height = Math.Min(img.Height - rect.Y, rect.Height + 2 * margin);

        return new Mat(img, rect);
    }
}