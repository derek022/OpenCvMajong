using Mahjong.Core.Util;
using Serilog;

namespace Mahjong.Core;

public class GameBoard
{
    // 加上边界
    public Cards[] Boards = null!;
    public Dictionary<Cards,List<Vector2Int>> CardPositions = new Dictionary<Cards, List<Vector2Int>>();
    public MoveAction CurrentAction = null!;
    
    public int Width { get; set; }
    public int Height { get; set; }
    
    public void SetBoardData(Cards[,] initialBoard)
    {
        Width = initialBoard.GetLength(1) + 2;
        Height = initialBoard.GetLength(0) + 2;
        Log.Logger.Information($"width:{Width},height:{Height}");
        Boards = new Cards[Width * Height];
        Clear();
        for (int x = 0; x < initialBoard.GetLength(0); x++)
        {
            for (int y = 0; y < initialBoard.GetLength(1); y++)
            {
                // Log.Logger.Information($"try set value {x + 1}，{y + 1},{initialBoard[x, y]}");
                SetCard(x + 1, y + 1, initialBoard[x, y]);
            }
        }

        ForceUpdateCardCachePos();
    }

    public void Clear()
    {
        Array.Fill(Boards,Cards.Guard,0,Boards.Length);
        for (int i = 1; i < Width - 1; i++)
        {
            for (int j = 1; j < Height - 1; j++)
            {
                SetCard(i,j,Cards.Zero);
            }
        }
        CardPositions.Clear();
    }
    
    public void SetEmpty(Vector2Int pos)
    {
        SetEmpty(pos.x,pos.y);
    }

    private int Two2OnePos(int x, int y)
    {
        return y * Width + x;
    }

    public void SetEmpty(int x, int y)
    {
        Boards[Two2OnePos(x,y)] = Cards.Zero;
    }
    
    public void SetGuard(Vector2Int pos)
    {
        SetGuard(pos.x,pos.y);
    }
    
    public void SetGuard(int x, int y)
    {
        Boards[Two2OnePos(x,y)] = Cards.Guard;
    }
    
    public bool IsEmpty(Vector2Int pos)
    {
        return IsEmpty(pos.x, pos.y);
    }
    
    public bool IsEmpty(int x,int y)
    {
        return Boards[Two2OnePos(x,y)] == Cards.Zero;
    }
    
    public void SetCard(Vector2Int pos, Cards card)
    {
        SetCard(pos.x,pos.y,card);
    }

    
    public void SetCard(int x,int y, Cards card)
    {
        Boards[Two2OnePos(x,y)] = card;
    }

    public Cards GetCard(Vector2Int pos)
    {
        return GetCard(pos.x, pos.y);
    }

    public Cards GetCard(int x, int y)
    {
        return Boards[Two2OnePos(x,y)];
    }

    public void MergeCard(Vector2Int start, Vector2Int target)
    {
        var card = GetCard(target);
        if (CardPositions.TryGetValue(card, out List<Vector2Int> positions))
        {
            positions.Remove(start);
            positions.Remove(target);

            if (positions.Count == 0)
            {
                CardPositions.Remove(card);
            }
        }
        
        SetCard(start, Cards.Zero);
        SetCard(target, Cards.Zero);
    }

    /// <summary>
    /// 强制更新牌的缓存位置信息
    /// </summary>
    public void ForceUpdateCardCachePos()
    {
        CardPositions.Clear();
        for (int x = 1; x < Width - 1; x++)
        {
            for (int y = 1; y < Height - 1; y++)
            {
                var card = GetCard(x, y);
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

    public GameBoard DeepClone()
    {
        var gameBoard = new GameBoard();
        gameBoard.CardPositions = new Dictionary<Cards, List<Vector2Int>>(CardPositions);
        gameBoard.Boards = new Cards[Width * Height];
        gameBoard.Width = Width;
        gameBoard.Height = Height;
        gameBoard.CurrentAction = new MoveAction();
        Array.Copy(Boards,gameBoard.Boards,Boards.Length);
        return gameBoard;
    }
}