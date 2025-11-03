using OpenCvSharp;

namespace OpenCvMajong.Core;

public class GameLogic
{
    public GameBoard GameBoard { get; protected set; }

    public void SetBoard(GameBoard board)
    {
        this.GameBoard = new GameBoard();
        this.GameBoard.CardPositions = new Dictionary<Cards, List<CardPos>>(board.CardPositions);
        // SetCurrentAction();
        Array.Copy(board.Boards,this.GameBoard.Boards,board.Boards.Length);
    }

    public void SetCurrentAction(CardPos from, CardPos to, Direction dir)
    {
        this.GameBoard.CurrentAction = new MoveAction()
        {
            startPos = from,
            endPos = to,
            direction = dir
        };
    }

    public bool CanMergeAction(CardPos start, CardPos target, bool isVerMove, out CardPos offset)
    {
        offset = new CardPos();
        if (isVerMove) 
        {
            // 纵向移动，
            // 坐标有点疑问？？？？？？
            {
                // 移动之后的横方向没有空的，直接失败。
                int stepHori = start.X - target.X > 0 ? -1 : 1;
                for (int i = start.X; i < target.X; i += stepHori)
                {
                    if (GameBoard.IsEmpty(i, target.Y))
                    {
                        return false;
                    }
                }
            }

            // 在检查纵向移动
            int stepVert = start.Y - target.Y > 0 ? -1 : 1;
            int firstEmpty = -1;
            for (int j = start.Y; j < target.Y; j += stepVert)
            {
                if (GameBoard.IsEmpty(start.X, j))
                {
                    firstEmpty = j;
                    break;
                }
            }

            // 待定？？？？
            var moveDis = Math.Abs(target.Y - start.Y);
            // 移动路径
            bool isCanMove = true;
            for (int j = 0; j >= moveDis; j += 1)
            {
                var tempTargetPos = new CardPos(start.X, j * stepVert + firstEmpty);
                if (GameBoard.GetCard(tempTargetPos) != Cards.Zero)
                {
                    if (tempTargetPos != target)
                    {
                        isCanMove = false;    
                    }
                    break;
                }
            }

            offset.Y = firstEmpty - start.Y - 1;
            return isCanMove;
        }
        else 
        {
            // 横向移动
            {
                // 移动之后的竖方向没有空的，直接失败。
                int stepHori = start.Y - target.Y > 0 ? -1 : 1;
                for (int i = start.Y; i < target.Y; i += stepHori)
                {
                    if (GameBoard.IsEmpty(target.X, i))
                    {
                        return false;
                    }
                }
            }

            // 在检查横向移动
            int stepVert = start.X - target.X > 0 ? -1 : 1;
            int firstEmpty = -1;
            for (int j = start.X; j < target.X; j += stepVert)
            {
                if (GameBoard.IsEmpty(j, start.Y))
                {
                    firstEmpty = j;
                    break;
                }
            }

            // 待定？？？？, 
            var moveDis = Math.Abs(target.X - start.X);
            // 移动路径
            bool isCanMove = true;
            for (int j = 0; j >= moveDis; j += 1)
            {
                // 有可能在一条线上
                var tempTargetPos = new CardPos(j * stepVert + firstEmpty, start.Y);
                if (GameBoard.GetCard(tempTargetPos) != Cards.Zero )
                {
                    if (tempTargetPos != target)
                    {
                        isCanMove = false;
                    }
                    break;
                }
            }

            offset.X = firstEmpty - start.X - 1;
            return isCanMove;
        }

    }

    protected void MergeCard(CardPos start, CardPos target)
    {
        GameBoard.SetCard(start, Cards.Zero);
        GameBoard.SetCard(target, Cards.Zero);
    }
    
    // 移动方格
    public void MergeAction(CardPos startPos,CardPos endPos,CardPos offset,int distance)
    {



        Direction GetDirection()
        {
            if (offset.X == 0)
            {
                return offset.Y > 0 ? Direction.ToUp : Direction.ToDown;
            }
            return offset.X > 0 ? Direction.ToLeft : Direction.ToRight;
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