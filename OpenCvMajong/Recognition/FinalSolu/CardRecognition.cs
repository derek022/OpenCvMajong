using System.Diagnostics;
using Mahjong.Core;
using Mahjong.Core.Util;
using OpenCvSharp;
using Serilog;

namespace Mahjong.Recognition.FinalSolu;

public class CardRecognition
{
    protected static readonly ILogger Logger = Log.ForContext<CardRecognition>();
    
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

        Cards[,] initBoard = new Cards[12,10];
        var bigMat = new Mat(screenShot);
        foreach (var templateFilePath in Directory.GetFiles(templateDir,"*.png"))
        {
            // Logger.Debug($"查找：{templateFilePath}");
            using var template = new Mat(templateFilePath);
            var results = MahjongTemplateMatcher.FindAllUniqueMatches(bigMat, template, minScale, maxScale);
            
            var cardName = Path.GetFileNameWithoutExtension(templateFilePath);
            // 将这个枚举名称，转换为枚举，
            var cardEnum = Enum.Parse<Cards>(cardName);

            foreach (var pos in results)
            {
                var realPos = new Vector2Int(pos.X / 100 , (pos.Y - 500) / 100);

                if (initBoard[realPos.y, realPos.x] == Cards.Zero || initBoard[realPos.y, realPos.x] == cardEnum)
                {
                    initBoard[realPos.y, realPos.x] = cardEnum;    
                }
                else
                {
                    Logger.Error($"坐标转换出现错误，已有值{initBoard[realPos.y, realPos.x].ToString()}");
                }
            }
        }

        swTotal.Stop();
        Logger.Information($"总耗时: {swTotal.ElapsedMilliseconds} ms");

        return initBoard;
    }
}