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
            Cards.WestWind, Cards.OneBmb, Cards.Zero, Cards.WestWind, Cards.Zero, Cards.Zero,
        },
        {
            Cards.Zero, Cards.Zero, Cards.Bamboo, Cards.NorthWind, Cards.EastWind, Cards.WestWind,
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

    public static Cards[,] TestDead = new Cards[8, 6]
    {
        {
            Cards.OneBmb, Cards.Zero, Cards.Zero, Cards.Zero, Cards.Zero, Cards.Zero,
        },
        {
            Cards.WestWind, Cards.Zero, Cards.Zero, Cards.Zero, Cards.Zero, Cards.Zero,
        },
        {
            Cards.OneBmb, Cards.Zero, Cards.Zero, Cards.Zero, Cards.Zero, Cards.Zero,
        },
        {
            Cards.WestWind, Cards.Zero, Cards.Zero, Cards.Zero, Cards.Zero, Cards.Zero,
        },
        {
            Cards.Zero, Cards.Zero, Cards.Zero, Cards.Zero, Cards.Zero, Cards.Zero,
        },
        {
            Cards.Zero, Cards.Zero, Cards.Zero, Cards.Zero, Cards.Zero, Cards.Zero,
        },
        {
            Cards.Zero, Cards.Zero, Cards.Zero, Cards.Zero, Cards.Zero, Cards.Zero,
        },
        {
            Cards.Zero, Cards.Zero, Cards.Zero, Cards.Zero, Cards.Zero, Cards.Zero,
        },
    };
    
    public static Cards[,] Test2 = new Cards[8, 6]
    {
        {
            Cards.WestWind, Cards.TwoBmb, Cards.EastWind, Cards.GreenDrg, Cards.SixDots, Cards.SixRank,
        },
        {
            Cards.NorthWind, Cards.SevenRank, Cards.TwoBmb, Cards.NorthWind, Cards.SixRank, Cards.SixDots,
        },
        {
            Cards.WestWind, Cards.SixRank, Cards.SevenRank, Cards.SevenBmb, Cards.WhiteDrg, Cards.NorthWind,
        },
        {
            Cards.SixRank, Cards.WestWind, Cards.Zero, Cards.SixRank, Cards.ThreeDots, Cards.WhiteDrg,
        },
        {
            Cards.OneBmb, Cards.SevenBmb, Cards.Zero, Cards.WestWind, Cards.TwoBmb, Cards.EastWind,
        },
        {
            Cards.ThreeDots, Cards.EastWind, Cards.SixDots, Cards.SixDots, Cards.SixRank, Cards.WhiteDrg,
        },
        {
            Cards.OneBmb, Cards.EastWind, Cards.SevenBmb, Cards.NorthWind, Cards.GreenDrg, Cards.WhiteDrg,
        },
        {
            Cards.GreenDrg, Cards.OneBmb, Cards.SevenBmb, Cards.OneBmb, Cards.TwoBmb, Cards.GreenDrg,
        },
    };
}