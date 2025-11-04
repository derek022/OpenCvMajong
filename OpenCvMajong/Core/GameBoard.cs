using Mahjong.Core.Util;
using OpenCvMajong;
using OpenCvMajong.Core;

namespace Mahjong.Core;

public class GameBoard
{
    // 加上边界
    public Cards[] Boards = null!;
    public Dictionary<Cards,List<Vector2Int>> CardPositions = new Dictionary<Cards, List<Vector2Int>>();
    public MoveAction CurrentAction = null!;
    
    public int Width { get; private set; }
    public int Height { get; private set; }
    public void InitBoard(Cards[,] initialBoard)
    {
        Width = initialBoard.GetLength(1) + 2;
        Height = initialBoard.GetLength(0) + 2;
        Console.WriteLine($"width:{Width},height:{Height}");
        Boards = new Cards[Width * Height];
        Clear();
        for (int x = 0; x < initialBoard.GetLength(0); x++)
        {
            for (int y = 0; y < initialBoard.GetLength(1); y++)
            {
                Console.WriteLine($"try set value {x}，{y},{initialBoard[x, y]}");
                SetCard(x, y, initialBoard[x, y]);

                if (initialBoard[x, y] != Cards.Zero)
                {
                    if (!CardPositions.ContainsKey(initialBoard[x, y]))
                    {
                        CardPositions[initialBoard[x, y]] = new List<Vector2Int>();
                    }

                    CardPositions[initialBoard[x, y]].Add(new Vector2Int (  x + 1, y + 1 ));
                }
            }
        }
    }

    public void Clear()
    {
        Array.Fill(Boards,Cards.Guard,0,Boards.Length);
        CardPositions.Clear();
    }
    
    
    public void SetEmpty(Vector2Int pos)
    {
        SetEmpty(pos.x,pos.y);
    }

    public void SetEmpty(int x, int y)
    {
        Boards[x * Height + y] = Cards.Zero;
    }
    
    public void SetGuard(Vector2Int pos)
    {
        SetGuard(pos.x,pos.y);
    }
    
    public void SetGuard(int x, int y)
    {
        Boards[x * Height + y] = Cards.Guard;
    }
    
    public bool IsEmpty(Vector2Int pos)
    {
        return IsEmpty(pos.x, pos.y);
    }
    
    public bool IsEmpty(int posX,int posY)
    {
        return Boards[posX * Height + posY] == Cards.Zero;
    }
    
    public void SetCard(Vector2Int pos, Cards card)
    {
        SetCard(pos.x,pos.y,card);
    }

    
    public void SetCard(int posX,int posY, Cards card)
    {
        Boards[posX * Height + posY] = card;
    }

    public Cards GetCard(Vector2Int pos)
    {
        return GetCard(pos.x, pos.y);
    }

    public Cards GetCard(int posX, int posY)
    {
        return Boards[posX * Height + posY];
    }

}