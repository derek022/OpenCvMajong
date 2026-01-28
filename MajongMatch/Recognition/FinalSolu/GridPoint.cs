namespace Mahjong.Recognition.FinalSolu;

// 定义一个用于哈希的结构，表示一个区域的坐标
public readonly struct GridPoint : IEquatable<GridPoint>
{
    public readonly int X;
    public readonly int Y;

    public GridPoint(int x, int y)
    {
        X = x;
        Y = y;
    }

    public bool Equals(GridPoint other)
    {
        return X == other.X && Y == other.Y;
    }

    public override bool Equals(object obj)
    {
        return obj is GridPoint other && Equals(other);
    }

    public override int GetHashCode()
    {
        // 使用移位运算是一种常见的组合两个整数哈希值的方法
        return HashCode.Combine(X, Y);
    }
}