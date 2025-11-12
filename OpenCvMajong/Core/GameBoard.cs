using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using Mahjong.Core.Util;
using Serilog;

namespace Mahjong.Core;

public class GameBoard
{
    // 加上边界
    public Cards[] Boards { get; set; } = null!;

    public MoveAction CurrentAction { get; set; } = null;

    public int Width { get; set; }
    public int Height { get; set; }
    
    public void SetBoardData(Cards[,] initialBoard)
    {
        Width = initialBoard.GetLength(1) + 2;
        Height = initialBoard.GetLength(0) + 2;
        Log.Information($"width:{Width},height:{Height}");
        Boards = new Cards[Width * Height];
        Clear();
        for (int x = 0; x < initialBoard.GetLength(0); x++)
        {
            for (int y = 0; y < initialBoard.GetLength(1); y++)
            {
                SetCard(y + 1, x + 1, initialBoard[x, y]);
            }
        }
    }

    public void Clear()
    {
        Array.Fill(Boards, Cards.Zero, 0, Width * Height);
        for (int i = 0; i < Width ; i++)
        {
            for (int j = 0; j < Height; j++)
            {
                if(i == 0 || i == Width - 1 || j == 0 || j == Height - 1)
                    SetCard(i,j,Cards.Guard);
            }
        }
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int Two2OnePos(int x, int y)
    {
        if ((uint)x > Width || (uint)y > Height)
        {
            throw new IndexOutOfRangeException($"x:{x},y:{y}");
        }
        return y * Width + x;
    }
    
    public void SetEmpty(Vector2Int pos)
    {
        SetEmpty(pos.x,pos.y);
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

    public bool IsGuard(Vector2Int pos)
    {
        return IsGuard(pos.x, pos.y);
    }

    public bool IsGuard(int x, int y)
    {
        return Boards[Two2OnePos(x,y)] == Cards.Guard;
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
        SetCard(start, Cards.Zero);
        SetCard(target, Cards.Zero);
    }

    public GameBoard DeepClone()
    {
        // var gameBoard = new GameBoard();
        // gameBoard.Boards = new Cards[Width * Height];
        // gameBoard.Width = Width;
        // gameBoard.Height = Height;
        // Array.Copy(Boards, gameBoard.Boards, Width * Height);
        // return gameBoard;
        return Tools.DeepCopy(this);
    }
    
    public void PrintState()
    {
        var curAction = CurrentAction;
        if (curAction != null)
        {
            Log.Information($"start: {curAction.StartPos} ,direction: {curAction.Direction}, target:{curAction.EndPos}");
        }
        Log.Information("Game Mahjong States is :");
        for (int i = 0; i < Height; i++)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append($"{i,3}:row => ");
            for (int j = 0; j < Width ; j++)
            {
                builder.Append($"{GetCard(j, i),10}");
            }
            Log.Information(builder.ToString());
        }
        Log.Information("-------------------------");
    }
}