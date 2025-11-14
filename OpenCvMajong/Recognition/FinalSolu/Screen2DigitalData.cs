using System.Diagnostics;
using System.Numerics;
using Mahjong.Core;
using Mahjong.Core.Util;
using OpenCvSharp;
using Serilog;

namespace Mahjong.Recognition.FinalSolu;

public class Screen2DigitalData
{
    public static Cards[,] Execute(string screenShot, string templateDir, float minScale,float maxScale)
    {
        Cards[,] initBoard = new Cards[12,10];
        foreach (var templateFilePath in Directory.GetFiles(templateDir,"*.png"))
        {
            Log.Debug($"查找：{templateFilePath}");
            using var template = new Mat(templateFilePath);
            var results = MahjongTemplateMatcher.FindAllUniqueMatches(screenShot, template, minScale, maxScale);
            
            var cardName = Path.GetFileNameWithoutExtension(templateFilePath);
            // 将这个枚举名称，转换为枚举，
            var cardEnum = Enum.Parse<Cards>(cardName);

            if (results.Count % 2 != 0)
            {
                MahjongTemplateMatcher.DrawMatches(screenShot, results, template,
                    "result/" + Path.GetFileName(templateFilePath));
            }
            foreach (var pos in results)
            {
                var realPos = new Vector2Int(pos.X / 100 , (pos.Y - 500) / 100);
                Log.Debug($"坐标转换：{cardEnum.ToString()},screenPos:{pos.X}_{pos.Y} , realPos:{realPos}");

                if (initBoard[realPos.y, realPos.x] == Cards.Zero)
                {
                    initBoard[realPos.y, realPos.x] = cardEnum;    
                }
                else
                {
                    Log.Error($"坐标转换出现错误，已有值{initBoard[realPos.y, realPos.x].ToString()}");
                }
                
            }
        }

        for (int i = 0; i < initBoard.GetLength(0); i++)
        {
            for (int j = 0; j < initBoard.GetLength(1); j++)
            {
                if (initBoard[i, j] == Cards.Zero)
                {
                    Log.Error($"{i},{j} is zero");
                }
            }
        }
        
        return initBoard;
    }
}