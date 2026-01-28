namespace SudokuSolver;

public class SudokuCell
{
    public int Num { get; set; }
    public bool Fixed { get; set; }
    public HashSet<int> Candidators { get; set; } = new HashSet<int>();

    public SudokuCell Clone()
    {
        return new SudokuCell
        {
            Num = this.Num,
            Fixed = this.Fixed,
            Candidators = new HashSet<int>(this.Candidators)
        };
    }
}