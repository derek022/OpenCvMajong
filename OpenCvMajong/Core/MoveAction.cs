using Mahjong.Core.Util;

namespace Mahjong.Core;

public class MoveAction
{
    public Vector2Int StartPos { get; set; } = null!;
    public Vector2Int EndPos { get; set; }  = null!;
    public Direction Direction { get; set; }
    public Vector2Int? Offset { get; set; }
    public int Distance { get; set; }
}
