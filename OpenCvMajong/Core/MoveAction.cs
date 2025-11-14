using Mahjong.Core.Util;

namespace Mahjong.Core;

public class MoveAction
{
    public Vector2Int StartPos { get; set; }
    public Vector2Int EndPos { get; set; }
    public Direction Direction { get; set; }
}