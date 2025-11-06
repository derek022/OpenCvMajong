using System.Text;
using Mahjong.Core.Util;
using Serilog;

namespace Mahjong.Core;

public class GameLogic
{
    public GameBoard GameBoard { get; protected set; }
    
    public Dictionary<Cards,List<Vector2Int>> CardPositions = new Dictionary<Cards, List<Vector2Int>>();

    public GameLogic(GameBoard gameBoard)
    {
        SetBoard(gameBoard);
    }

    public void SetBoard(GameBoard board)
    {
        this.GameBoard = board;
        ForceUpdateCardCachePos();
    }

    public void SetCurrentAction(Vector2Int from, Vector2Int to, Direction dir)
    {
        this.GameBoard.CurrentAction = new MoveAction()
        {
            StartPos = from,
            EndPos = to,
            Direction = dir
        };
    }

    private bool IsVerticalTwoPointEmpty(Vector2Int start, Vector2Int target)
    {
        int min = start.y > target.y ? target.y : start.y;
        int max = start.y < target.y ? target.y : start.y;
        // int stepHori = start.y - target.y > 0 ? -1 : 1;
        for (int i = min + 1; i < max; i += 1)
        {
            if (GameBoard.IsEmpty(target.x, i) == false)
            {
                return false;
            }
        }
        return true;
    }
    private bool IsHorizontalTwoPointEmpty(Vector2Int start, Vector2Int target)
    {
        int min = start.x > target.x ? target.x : start.x;
        int max = start.x < target.x ? target.x : start.x;
        for (int i = min + 1; i < max; i += 1)
        {
            if (GameBoard.IsEmpty(i,target.y) == false)
            {
                return false;
            }
        }
        return true;
    }

    public bool CanMergeAction(Vector2Int start, Vector2Int target, bool isVerMove, out Vector2Int offset)
    {
        // Log.Debug($"检查移动方向:start:{start},target{target},isVer:{isVerMove}");
        offset = new Vector2Int(0, 0);
        if (start.x == target.x)
        {
            return IsVerticalTwoPointEmpty(start, target);
        }
        else if (start.y == target.y)
        {
            return IsHorizontalTwoPointEmpty(start, target);
        }
        else if (isVerMove)
        {
            // 纵向移动，
            {
                // 移动之后的横方向没有空的，直接失败。
                if (IsHorizontalTwoPointEmpty(start, target) == false)
                    return false;
            }

            if (start.y == target.y)
            {
                return true;
            }

            // 在检查纵向移动
            int stepVert = start.y - target.y > 0 ? -1 : 1;
            int firstEmpty = start.y + stepVert;
            while (true)
            {
                if (GameBoard.IsEmpty(start.x, firstEmpty))
                {
                    break;
                }

                if (GameBoard.IsGuard(start.x, firstEmpty))
                {
                    return false;
                }

                firstEmpty += stepVert;
            }

            // 待定？？？？
            var moveDis = Math.Abs(target.y - start.y);
            // 移动路径
            bool isCanMove = true;

            for (int j = 1; j < moveDis; j += 1)
            {
                var tempTargetPos = new Vector2Int(start.x, j * stepVert + firstEmpty);
                var targetCard = GameBoard.GetCard(tempTargetPos);
                if (targetCard != Cards.Zero)
                {
                    if (tempTargetPos != target)
                    {
                        isCanMove = false;
                    }

                    break;
                }

            }

            offset.y = firstEmpty - start.y - stepVert;
            return isCanMove;
        }
        else
        {
            // 横向移动
            {
                // 移动之后的竖方向没有空的，直接失败。
                if (IsVerticalTwoPointEmpty(start, target) == false)
                    return false;
            }

            if (start.x == target.x)
            {
                return true;
            }

            // 在检查横向移动
            int stepHor = start.x - target.x > 0 ? -1 : 1;
            int firstEmpty = start.x + stepHor;
            while (true)
            {
                if (GameBoard.IsEmpty(firstEmpty, start.y))
                {
                    break;
                }

                if (GameBoard.IsGuard(firstEmpty, start.y))
                {
                    return false;
                }

                firstEmpty += stepHor;
            }

            // 偏移的距离大小 
            var moveDis = Math.Abs(target.x - start.x);
            // 移动路径
            bool isCanMove = true;
            for (int j = 1; j < moveDis; j += 1)
            {
                // 有可能在一条线上
                var tempTargetPos = new Vector2Int(j * stepHor + firstEmpty, start.y);
                var targetCard = GameBoard.GetCard(tempTargetPos);
                if (targetCard != Cards.Zero)
                {
                    if (tempTargetPos != target)
                    {
                        isCanMove = false;
                    }

                    break;
                }
            }

            offset.x = firstEmpty - start.x - stepHor;
            return isCanMove;
        }

    }

    // 移动方格
    public void MergeAction(Vector2Int startPos,Vector2Int endPos,Vector2Int offset,int distance)
    {
        // Log.Logger.Information($"检测到可以移动的方块,start:{startPos},end:{endPos},offset:{offset},distance:{distance}");
        // 移动多少个，还有向量的方向。
        if (offset != Vector2Int.zero)
        {
            var moveCnt = (int)offset.magnitude;

            var normalVector = offset / moveCnt;

            for (int i = moveCnt ; i > 0; i--)
            {
                var pos = startPos + normalVector * i;
                var end = startPos + normalVector * (i + distance);
                // Log.Debug($"moving: pos{pos},endPos{end}");
                GameBoard.SetCard(end,GameBoard.GetCard(pos));
                GameBoard.SetCard(pos, Cards.Zero);
            }
        }
        
        GameBoard.MergeCard(startPos,endPos);

        // 更新卡牌的缓存位置信息
        ForceUpdateCardCachePos();
        
        Direction GetDirection()
        {
            if (offset == Vector2Int.zero)
            {
                return Direction.None;
            }
            if (offset.x == 0)
            {
                return offset.y > 0 ? Direction.ToDown : Direction.ToUp;
            }
            return offset.x > 0 ? Direction.ToRight : Direction.ToLeft;
        }
        
        SetCurrentAction(startPos,endPos,GetDirection());
    }

    public bool IsFinalState()
    {
        return CardPositions.Count == 0;
    }

    public void PrintState()
    {
        GameBoard?.PrintState();
    }
    
    
    /// <summary>
    /// 强制更新牌的缓存位置信息
    /// </summary>
    public void ForceUpdateCardCachePos()
    {
        CardPositions.Clear();
        for (int x = 1; x < GameBoard.Width - 1; x++)
        {
            for (int y = 1; y < GameBoard.Height - 1; y++)
            {
                var card = GameBoard.GetCard(x, y);
                if (card != Cards.Zero)
                {
                    if (!CardPositions.ContainsKey(card))
                    {
                        CardPositions[card] = new List<Vector2Int>();
                    }

                    CardPositions[card].Add(new Vector2Int(x, y));
                }
            }
        }
    }
}