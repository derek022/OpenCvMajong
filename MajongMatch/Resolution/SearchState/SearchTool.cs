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

        //todo 逻辑有问题
        //1. 筛选水平或垂直移动，并同时去重
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
        readMoves.Sort(new MoveActionDistanceComparer());

        return readMoves;
    }
    
    
    
    /// <summary>
    /// 部分匹配，死局判定辅助方法
    /// </summary>
    /// <param name="array1"></param>
    /// <param name="subArray"></param>
    /// <param name="wildcardValue"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static bool ArrayMatchesWithWildcard<T>(T[] array1, T[] subArray, T wildcardValue)
    {
        if (array1 == null || subArray == null || array1.Length != subArray.Length)
            return false;

        for (int i = 0; i < array1.Length; i++)
        {
            // 如果 array2[i] 是通配符，则跳过比较
            if (subArray[i].Equals(wildcardValue))
                continue;

            // 如果 array2[i] 不是通配符，则必须与 array1[i] 相等
            if (!array1[i].Equals(subArray[i]))
            {
                return false;
            }
        }

        return true;
    }

}