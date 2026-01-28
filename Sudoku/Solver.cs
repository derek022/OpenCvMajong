namespace SudokuSolver;

public class Solver
{

    static int _ddd = 0;
    static int _rc = 0;

    static int RoundBlockNumber(int row, int col)
    {
        return (row / SudokuGame.SkdBlockColLimit) * SudokuGame.SkdBlockRowLimit + (col / SudokuGame.SkdBlockColLimit);
    }

    internal static void InitSudokuGame(SudokuGame game, int[] numbers)
    {
        game.FixedCount = 0;
        for (int i = 0; i < SudokuGame.SkdCellCount; i++)
        {
            int row = i / SudokuGame.SkdColLimit;
            int col = i % SudokuGame.SkdColLimit;
            int num = numbers[i];

            game.Cells[row, col] = new SudokuCell
            {
                Num = num,
                Fixed = num != 0
            };

            if (!game.Cells[row, col].Fixed)
            {
                for (int n = 1; n <= 9; n++) game.Cells[row, col].Candidators.Add(n);
            }
            else
            {
                game.FixedCount++;
            }
        }
    }

    static bool CheckValidCandidator(SudokuGame game, int row, int col, int num)
    {
        for (int i = 0; i < SudokuGame.SkdColLimit; i++)
            if (i != col && game.Cells[row, i].Num == num)
                return false;

        for (int j = 0; j < SudokuGame.SkdRowLimit; j++)
            if (j != row && game.Cells[j, col].Num == num)
                return false;

        int rs = (row / SudokuGame.SkdBlockRowLimit) * SudokuGame.SkdBlockRowLimit;
        int cs = (col / SudokuGame.SkdBlockColLimit) * SudokuGame.SkdBlockColLimit;

        for (int r = rs; r < rs + SudokuGame.SkdBlockColLimit; r++)
        for (int c = cs; c < cs + SudokuGame.SkdBlockRowLimit; c++)
            if (!(r == row && c == col) && game.Cells[r, c].Num == num)
                return false;

        return true;
    }

    static bool SetCandidatorToFixed(SudokuGame game, int row, int col, int num)
    {
        if (!CheckValidCandidator(game, row, col, num)) return false;

        var cell = game.Cells[row, col];
        cell.Num = num;
        cell.Fixed = true;
        cell.Candidators.Clear();

        if (!SingleRowExclusive(game, row, num)) return false;
        if (!SingleColumnExclusive(game, col, num)) return false;
        if (!SingleBlockExclusive(game, RoundBlockNumber(row, col), num)) return false;

        game.FixedCount++;
        return true;
    }

    static bool RemoveCellCandidator(SudokuGame game, int row, int col, int num)
    {
        var cell = game.Cells[row, col];
        if (!cell.Fixed)
        {
            cell.Candidators.Remove(num);
            if (cell.Candidators.Count == 1)
            {
                // 相当于 C++ 中的 SinglesCandidature
                int lastNum = cell.Candidators.First();
                if (!SetCandidatorToFixed(game, row, col, lastNum)) return false;
            }
            else if (cell.Candidators.Count == 0) return false; // 无解
        }

        return true;
    }

    static bool SingleRowExclusive(SudokuGame game, int row, int num)
    {
        for (int i = 0; i < SudokuGame.SkdColLimit; i++)
            if (!game.Cells[row, i].Fixed && game.Cells[row, i].Candidators.Contains(num))
                if (!RemoveCellCandidator(game, row, i, num))
                    return false;
        return true;
    }

    static bool SingleColumnExclusive(SudokuGame game, int col, int num)
    {
        for (int i = 0; i < SudokuGame.SkdRowLimit; i++)
            if (!game.Cells[i, col].Fixed && game.Cells[i, col].Candidators.Contains(num))
                if (!RemoveCellCandidator(game, i, col, num))
                    return false;
        return true;
    }

    static bool SingleBlockExclusive(SudokuGame game, int block, int num)
    {
        int rs = (block / SudokuGame.SkdBlockRowLimit) * SudokuGame.SkdBlockColLimit;
        int cs = (block % SudokuGame.SkdBlockRowLimit) * SudokuGame.SkdBlockRowLimit;

        for (int i = 0; i < 9; i++)
        {
            int row = rs + i / 3;
            int col = cs + i % 3;
            if (!game.Cells[row, col].Fixed && game.Cells[row, col].Candidators.Contains(num))
                if (!RemoveCellCandidator(game, row, col, num))
                    return false;
        }

        return true;
    }

    internal static void BaseExclusive(SudokuGame game)
    {
        for (int r = 0; r < SudokuGame.SkdRowLimit; r++)
        {
            for (int c = 0; c < SudokuGame.SkdColLimit; c++)
            {
                if (game.Cells[r, c].Fixed)
                {
                    int num = game.Cells[r, c].Num;
                    SingleRowExclusive(game, r, num);
                    SingleColumnExclusive(game, c, num);
                    SingleBlockExclusive(game, RoundBlockNumber(r, c), num);
                }
            }
        }
    }

    internal static void FindSudokuSolution(SudokuGame game, int sp)
    {
        if (game.FixedCount == SudokuGame.SkdCellCount)
        {
            _rc++;
            Console.WriteLine($"Find result {_rc} (ddd = {_ddd}):");
            PrintSudokuGame(game);
            return;
        }

        int row = sp / SudokuGame.SkdColLimit;
        int col = sp % SudokuGame.SkdColLimit;

        while (sp < SudokuGame.SkdCellCount && game.Cells[row, col].Fixed)
        {
            sp++;
            if (sp >= SudokuGame.SkdCellCount) break;
            row = sp / SudokuGame.SkdColLimit;
            col = sp % SudokuGame.SkdColLimit;
        }

        if (sp < SudokuGame.SkdCellCount)
        {
            var curCell = game.Cells[row, col];
            var candidates = curCell.Candidators.ToList(); // 复制一份迭代，防止修改冲突

            foreach (int val in candidates)
            {
                SudokuGame newState = game.Clone();
                if (SetCandidatorToFixed(newState, row, col, val))
                {
                    _ddd++;
                    FindSudokuSolution(newState, sp + 1);
                    _ddd--;
                }
            }
        }
    }

    internal static void PrintSudokuGame(SudokuGame game)
    {
        Console.WriteLine();
        for (int r = 0; r < SudokuGame.SkdRowLimit; r++)
        {
            for (int c = 0; c < SudokuGame.SkdColLimit; c++)
            {
                Console.Write(game.Cells[r, c].Num + "    ");
            }

            Console.WriteLine("\n");
        }
    }
}