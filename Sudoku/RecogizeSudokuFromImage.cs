using OpenCvSharp;
using OpenCvSharp.Extensions;
using Tesseract;
using Rect = OpenCvSharp.Rect;

namespace SudokuSolver;

public class RecogizeSudokuFromImage
{
    /// <summary>
    /// 使用 OpenCV 识别图片中的数独，返回一个一维数组（81个元素，0表示空格）
    /// </summary>
    public static int[] RecognizeSudokuFromImage(string imgPath)
    {

        int[] grid = new int[9 * 9];

        // 1. 加载并预处理图片
        using var src = Cv2.ImRead(imgPath);
        using var gray = src.CvtColor(ColorConversionCodes.BGR2GRAY);
        using var thresh = gray.AdaptiveThreshold(255, AdaptiveThresholdTypes.MeanC, ThresholdTypes.BinaryInv, 11, 2);

        // 2. 找到最大的正方形（数独盘面）
        var contours = Cv2.FindContoursAsArray(thresh, RetrievalModes.External, ContourApproximationModes.ApproxSimple);
        var mainGrid = contours.OrderByDescending(c => Cv2.ContourArea(c)).First();
        var rect = Cv2.BoundingRect(mainGrid);
        using var sudokuArea = new Mat(src, rect);

        // 3. 初始化 OCR 引擎
        // 请确保路径下有 tessdata 文件夹
        using var ocr = new TesseractEngine(@"./tessdata", "eng", EngineMode.Default);
        ocr.SetVariable("tessedit_char_whitelist", "123456789"); // 只识别数字

        int cellW = rect.Width / 9;
        int cellH = rect.Height / 9;

        for (int r = 0; r < 9; r++)
        {
            for (int c = 0; c < 9; c++)
            {
                // 切分每个小格子 (稍微向内缩进 5 像素以避开边框)
                var cellRect = new Rect(c * cellW + 5, r * cellH + 5, cellW - 10, cellH - 10);
                using var cell = new Mat(sudokuArea, cellRect);

                // 判断格子是否为空（计算黑色像素占比）
                if (IsCellEmpty(cell))
                {
                    grid[r * 9 + c] = 0;
                }
                else
                {
                    // OCR 识别
                    using var pix = PixConverter.ToPix(cell.ToBitmap());
                    using var page = ocr.Process(pix);
                    int.TryParse(page.GetText().Trim(), out grid[r * 9 + c]);
                }
            }
        }

        return grid;
    }

    private static bool IsCellEmpty(Mat cell)
        {
            // 将格子转为二值图并统计非零像素
            using var binary = cell.CvtColor(ColorConversionCodes.BGR2GRAY).Threshold(0, 255, ThresholdTypes.BinaryInv | ThresholdTypes.Otsu);
            double count = Cv2.CountNonZero(binary);
            double total = cell.Width * cell.Height;
            return (count / total) < 0.02; // 如果有内容的像素少于 2%，视为空格
        }

}