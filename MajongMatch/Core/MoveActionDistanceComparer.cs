namespace Mahjong.Core;


/// <summary>
/// 距离比较器
/// </summary>
public class MoveActionDistanceComparer : Comparer<MoveAction>
{
    public override int Compare(MoveAction? x, MoveAction? y)
    {
        if(x == null)
            return -1;
        if(y == null)
            return 1;
        return x.Distance.CompareTo(y.Distance);
    }
}
