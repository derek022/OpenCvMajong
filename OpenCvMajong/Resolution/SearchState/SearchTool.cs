using Mahjong.Core;
using Mahjong.Core.Util;

namespace Mahjong.Resolution.SearchState;

public static class SearchTool
{
    public static IEnumerable<bool> GetSearchDirArr(Vector2Int from, Vector2Int to)
    {
        if (from.x == to.x)
        {
            yield return false;
        }
        else if (from.y == to.y)
        {
            yield return true;
        }
        else
        {
            yield return true;
            yield return false;
        }
    }

    public static List<MoveAction> GeneratePossibleMoves(GameLogic current)
    {
        var moves = new List<MoveAction>();

        foreach (var pair in current.CardPositions)
        {
            var values = pair.Value;
            for (int i = 0; i < values.Count; i++)
            {
                for (int j = 0; j < values.Count; j++)
                {
                    if (i == j) continue;

                    var from = values[i];
                    var to = values[j];

                    foreach (var isVer in GetSearchDirArr(from, to))
                    {
                        if (current.CanMergeAction(from, to, isVer, out var offset, out var distance))
                        {
                            moves.Add(new MoveAction
                            {
                                StartPos = from,
                                EndPos = to,
                                Offset = offset,
                                Distance = distance,
                                Direction = Tools.GetDir(from, to, isVer)
                            });
                        }
                    }
                }
            }
        }

        // todo 可以根据某些启发式规则对moves排序，优先尝试更可能成功的移动
        // 1. 筛选水平或垂直移动，并同时去重
        var uniqueMoves = new HashSet<MoveAction>(new MoveActionReverseEqualityComparer());

        foreach (var moveAction in moves)
        {
            if (moveAction.StartPos.x == moveAction.EndPos.x || moveAction.StartPos.y == moveAction.EndPos.y)
            {
                uniqueMoves.Add(moveAction); // HashSet.Add 如果元素已存在，会返回 false，但不会报错
            }
        }

        // 2. 转换为列表并排序
        var readMoves = uniqueMoves.ToList(); // ToList 会创建一个新列表，可以安全排序
        // 排序，根据距离
        readMoves.Sort(new DistanceMoveAction());

        return readMoves;
    }
}