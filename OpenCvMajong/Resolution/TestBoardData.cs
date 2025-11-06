using Mahjong.Core.Util;

namespace Mahjong.Resolution;

public class TestBoardData
{
    public static Cards[,] Test1 = new Cards[8, 6]
    {
        {
            Cards.WestWind, Cards.WestWind, Cards.Zero, Cards.Zero, Cards.Zero, Cards.Zero,
        },
        {
            Cards.WestWind, Cards.OneBmb, Cards.Zero, Cards.Zero, Cards.Zero, Cards.Zero,
        },
        {
            Cards.OneBmb, Cards.Zero, Cards.Zero, Cards.Zero, Cards.Zero, Cards.Zero,
        },
        {
            Cards.Zero, Cards.Zero, Cards.WestWind, Cards.Zero, Cards.Zero, Cards.Zero,
        },
        {
            Cards.Zero, Cards.Zero, Cards.Zero, Cards.Zero, Cards.Zero, Cards.Zero,
        },
        {
            Cards.Zero, Cards.Zero, Cards.Zero, Cards.Zero, Cards.Zero, Cards.Zero,
        },
        {
            Cards.OneBmb, Cards.Zero, Cards.Zero, Cards.Zero, Cards.Zero, Cards.Zero,
        },
        {
            Cards.Zero, Cards.Zero, Cards.Zero, Cards.Zero, Cards.Zero, Cards.Zero,
        },
    };
}