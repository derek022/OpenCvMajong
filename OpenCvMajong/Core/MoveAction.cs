using Mahjong.Core.Util;

namespace Mahjong.Core;

public class MoveAction
{
    public Vector2Int StartPos { get; set; }
    public Vector2Int EndPos { get; set; }
    public Direction Direction { get; set; }
    public Vector2Int Offset { get; set; }
    public int Distance { get; set; }
}

/// <summary>
/// 距离比较器
/// </summary>
public class DistanceMoveAction : Comparer<MoveAction>
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


// 1. 定义一个比较器，用于判断移动是否“反向”相等
public class MoveActionReverseEqualityComparer : IEqualityComparer<MoveAction>
{
    public bool Equals(MoveAction x, MoveAction y)
    {
        // 检查是否是相同移动 (Start->End) 或反向移动 (End->Start)
        return (x.StartPos.Equals(y.StartPos) && x.EndPos.Equals(y.EndPos)) ||
               (x.StartPos.Equals(y.EndPos) && x.EndPos.Equals(y.StartPos));
    }

    public int GetHashCode(MoveAction obj)
    {
        // 为了反向相等，哈希码也必须相同。
        // 可以将两个点排序后生成哈希码
        var p1 = obj.StartPos;
        var p2 = obj.EndPos;
        // 确保顺序，例如 x 小的在前，x 相同时 y 小的在前
        if (p1.x > p2.x || (p1.x == p2.x && p1.y > p2.y))
        {
            (p1, p2) = (p2, p1);
        }
        return HashCode.Combine(p1.x, p1.y, p2.x, p2.y);
    }
}
