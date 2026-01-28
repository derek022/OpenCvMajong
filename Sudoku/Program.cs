using System;
using System.Collections.Generic;
using System.Linq;
using Serilog;
using SoluCore.Helper;

namespace SudokuSolver
{
    
    class Program
    {

        // 较难，最大递归深度11
        static int[] SudokuNumbers1 = 
        {
            0, 6, 0, 0, 2, 0, 0, 0, 0,
            0, 0, 0, 8, 0, 0, 5, 0, 0,
            0, 0, 1, 0, 0, 0, 0, 0, 0,
            7, 0, 0, 0, 3, 0, 2, 0, 0,
            0, 9, 0, 0, 0, 6, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 8, 0, 0,
            0, 0, 0, 9, 0, 1, 0, 6, 0,
            8, 0, 0, 4, 0, 0, 0, 0, 7,
            0, 0, 0, 0, 0, 0, 0, 0, 0
        };

        static void Main(string[] args)
        {
            SerilogHelper.InitLogger();
            
            var screenFile = "screen.png";
            
            // 如果没有图片，截图
            if (!File.Exists(screenFile))
            {
                InputHelper.Screenshot(screenFile);
            }
            
            Log.Debug($"开始识别数独图片: {screenFile}");
            Log.Debug($"完整路径: {Path.GetFullPath(screenFile)}");
            
            // 使用OpenCV识别数独
            var recognizedNumbers = RecogizeSudokuFromImage.RecognizeSudokuFromImage(screenFile);
            
            Log.Debug("\n识别结果:");
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    int value = recognizedNumbers[i * 9 + j];
                    Console.Write(value == 0 ? ". " : value + " ");
                }
                Log.Debug("");
            }

            return;
            // 创建游戏并求解
            SudokuGame game = new SudokuGame();
            Solver.InitSudokuGame(game, recognizedNumbers);
            Log.Debug("\n开始求解...");
            Solver.PrintSudokuGame(game);

            Solver.BaseExclusive(game);
            Solver.FindSudokuSolution(game, 0);

            Log.Debug("\nPress any key to exit...");
            Console.ReadKey();
        }

    }
}