namespace SudokuSolver;


public class SudokuGame
{
    public const int SkdRowLimit = 9;
    public const int SkdColLimit = 9;
    public const int SkdBlockRowLimit = 3;
    public const int SkdBlockColLimit = 3;
    public const int SkdCellCount = SkdRowLimit * SkdColLimit;

    public SudokuCell[,] Cells = new SudokuCell[SkdRowLimit, SkdColLimit];
    public int FixedCount { get; set; }

    public SudokuGame Clone()
    {
        var newGame = new SudokuGame { FixedCount = this.FixedCount };
        for (int r = 0; r < SkdRowLimit; r++)
        for (int c = 0; c < SkdColLimit; c++)
            newGame.Cells[r, c] = this.Cells[r, c].Clone();
        return newGame;
    }
}