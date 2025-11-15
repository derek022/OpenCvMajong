namespace Mahjong.Core;


// 1. 定义一个比较器，用于判断移动是否“反向”相等
public class MoveActionReverseEqualityComparer : IEqualityComparer<MoveAction>
{
    public bool Equals(MoveAction x, MoveAction y)
    {
        // 检查是否是相同移动 (Start->End) 或反向移动 (End->Start)
        if ((x.StartPos.Equals(y.StartPos) && x.EndPos.Equals(y.EndPos)) ||
            (x.StartPos.Equals(y.EndPos) && x.EndPos.Equals(y.StartPos)))
        {
            if (x.Offset == y.Offset)
            {
                if (x.Offset != null && x.Offset == Vector2Int.zero)
                {
                    return true;
                }
            }
            
        }

        return false;
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