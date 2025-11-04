using Mahjong.Core;
using Mahjong.Core.Util;

namespace OpenCvMajong.Core;

public class GameLogic
{
    public GameBoard GameBoard { get; protected set; }

    public void SetBoard(GameBoard board)
    {
        this.GameBoard = new GameBoard();
        this.GameBoard.CardPositions = new Dictionary<Cards, List<Vector2Int>>(board.CardPositions);
        // SetCurrentAction();
        GameBoard.Boards = new Cards[board.Width * board.Height];
        Array.Copy(board.Boards,this.GameBoard.Boards,board.Boards.Length);
    }

    public void SetCurrentAction(Vector2Int from, Vector2Int to, Direction dir)
    {
        this.GameBoard.CurrentAction = new MoveAction()
        {
            startPos = from,
            endPos = to,
            direction = dir
        };
    }

    public bool CanMergeAction(Vector2Int start, Vector2Int target, bool isVerMove, out Vector2Int offset)
    {
        offset = new Vector2Int(0, 0);
        if (isVerMove) 
        {
            // 纵向移动，
            // 坐标有点疑问？？？？？？
            {
                // 移动之后的横方向没有空的，直接失败。
                int stepHori = start.x - target.x > 0 ? -1 : 1;
                for (int i = start.x; i < target.x; i += stepHori)
                {
                    if (GameBoard.IsEmpty(i, target.y))
                    {
                        return false;
                    }
                }
            }

            // 在检查纵向移动
            int stepVert = start.y - target.y > 0 ? -1 : 1;
            int firstEmpty = -1;
            for (int j = start.y; j < target.y; j += stepVert)
            {
                if (GameBoard.IsEmpty(start.x, j))
                {
                    firstEmpty = j;
                    break;
                }
            }

            // 待定？？？？
            var moveDis = Math.Abs(target.y - start.y);
            // 移动路径
            bool isCanMove = true;
            for (int j = 0; j >= moveDis; j += 1)
            {
                var tempTargetPos = new Vector2Int(start.x, j * stepVert + firstEmpty);
                if (GameBoard.GetCard(tempTargetPos) != Cards.Zero)
                {
                    if (tempTargetPos != target)
                    {
                        isCanMove = false;    
                    }
                    break;
                }
            }

            offset.y = firstEmpty - start.y - 1;
            return isCanMove;
        }
        else 
        {
            // 横向移动
            {
                // 移动之后的竖方向没有空的，直接失败。
                int stepHori = start.y - target.y > 0 ? -1 : 1;
                for (int i = start.y; i < target.y; i += stepHori)
                {
                    if (GameBoard.IsEmpty(target.x, i))
                    {
                        return false;
                    }
                }
            }

            // 在检查横向移动
            int stepVert = start.x - target.x > 0 ? -1 : 1;
            int firstEmpty = -1;
            for (int j = start.x; j < target.x; j += stepVert)
            {
                if (GameBoard.IsEmpty(j, start.y))
                {
                    firstEmpty = j;
                    break;
                }
            }

            // 待定？？？？, 
            var moveDis = Math.Abs(target.x - start.x);
            // 移动路径
            bool isCanMove = true;
            for (int j = 0; j >= moveDis; j += 1)
            {
                // 有可能在一条线上
                var tempTargetPos = new Vector2Int(j * stepVert + firstEmpty, start.y);
                if (GameBoard.GetCard(tempTargetPos) != Cards.Zero )
                {
                    if (tempTargetPos != target)
                    {
                        isCanMove = false;
                    }
                    break;
                }
            }

            offset.x = firstEmpty - start.x - 1;
            return isCanMove;
        }

    }

    protected void MergeCard(Vector2Int start, Vector2Int target)
    {
        GameBoard.SetCard(start, Cards.Zero);
        GameBoard.SetCard(target, Cards.Zero);
    }
    
    // 移动方格
    public void MergeAction(Vector2Int startPos,Vector2Int endPos,Vector2Int offset,int distance)
    {

        // 移动多少个，还有向量的方向。
        var moveCnt = (int)offset.magnitude;

        var normalVector = offset / moveCnt;

        for (int i = moveCnt - 1; i >= 0; i--)
        {
            var pos = startPos + normalVector * i;
            var end = startPos + normalVector * (i + distance);
            GameBoard.SetCard(end,GameBoard.GetCard(pos));
            GameBoard.SetCard(pos, Cards.Zero);
        }
        
        MergeCard(startPos,endPos);

        Direction GetDirection()
        {
            if (offset.x == 0)
            {
                return offset.y > 0 ? Direction.ToUp : Direction.ToDown;
            }
            return offset.x > 0 ? Direction.ToLeft : Direction.ToRight;
        }
        
        SetCurrentAction(startPos,endPos,GetDirection());
    }

    public bool IsFinalState()
    {
        return GameBoard.CardPositions.Count == 0;
    }

    public void PrintState()
    {
        var curAction = GameBoard.CurrentAction;
        Console.WriteLine($"start: {curAction.startPos} ,direction: {curAction.direction}, target:{curAction.endPos}");
        Console.WriteLine("Game Mahjong States is :");
        for (int i = 1; i < GameBoard.Width; i++)
        {
            for (int j = 1; j < GameBoard.Height; j++)
            {
                Console.Write($"{GameBoard.Boards[i*GameBoard.Width + j]:4}");
            }
            Console.WriteLine();
        }
        Console.WriteLine("-------------------------");
    }
}