using Mahjong.Core.Util;

namespace Mahjong.Resolution;

public partial class SampleBoards
{
    public class TestData
    {
        public static Cards[,] Test1 = new Cards[8, 6]
        {
            {
                Cards.Zero, Cards.WestWind, Cards.Zero, Cards.Zero, Cards.Zero, Cards.Zero,
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
            {
                Cards.Zero, Cards.Zero, Cards.Zero, Cards.Zero, Cards.Zero, Cards.Zero,
            },
            {
                Cards.Zero, Cards.Zero, Cards.Zero, Cards.Zero, Cards.Zero, Cards.Zero,
            },
        };

        public static Cards[,] TestDead2 = new Cards[12, 10]
        {
            {
                Cards.GreenDrg, Cards.FiveRank, Cards.TwoBmb, Cards.Zero, Cards.Zero, Cards.Zero, Cards.Zero, Cards.Zero, Cards.Zero, Cards.Zero
            },
            {
                Cards.Zero, Cards.Zero, Cards.Zero, Cards.Zero, Cards.Zero, Cards.Zero, Cards.Zero, Cards.Zero, Cards.Zero, Cards.Zero,
            },
            {
                Cards.Zero, Cards.Zero, Cards.Zero, Cards.Zero, Cards.Zero, Cards.Zero, Cards.Zero, Cards.Zero, Cards.Zero, Cards.Zero
            },
            {
                Cards.Zero, Cards.Zero, Cards.Zero, Cards.Zero, Cards.NorthWind, Cards.Zero, Cards.Zero, Cards.Zero, Cards.Zero, Cards.Zero,
            },
            {
                Cards.Zero, Cards.Zero, Cards.Zero, Cards.Zero, Cards.Zero, Cards.Zero, Cards.Zero, Cards.Zero, Cards.Zero, Cards.Zero,
            },
            {
                Cards.Zero, Cards.Zero, Cards.Zero, Cards.Zero, Cards.Zero, Cards.Zero, Cards.Zero, Cards.Zero, Cards.Zero, Cards.Zero,
            },
            {
                Cards.Zero, Cards.Zero, Cards.Zero, Cards.Zero, Cards.Zero, Cards.Zero, Cards.Zero, Cards.Zero, Cards.Zero, Cards.Zero,
            },
            {
                Cards.Zero, Cards.Zero, Cards.Zero, Cards.Zero, Cards.Zero, Cards.Zero, Cards.Zero, Cards.Zero, Cards.Zero, Cards.FiveRank
            },
            {
                Cards.Zero, Cards.Zero, Cards.Zero, Cards.Zero, Cards.Zero, Cards.Zero, Cards.Zero, Cards.Zero, Cards.Zero, Cards.Zero,
            },
            {
                Cards.Zero, Cards.Zero, Cards.Zero, Cards.Zero, Cards.Zero, Cards.Zero, Cards.Zero, Cards.Zero, Cards.TwoBmb, Cards.Zero,
            },
            {
                Cards.Zero, Cards.Zero, Cards.Zero, Cards.Zero, Cards.Zero, Cards.Zero, Cards.Zero, Cards.Zero, Cards.Zero, Cards.GreenDrg
            },

            {
                Cards.Zero, Cards.Zero, Cards.Zero, Cards.Zero, Cards.Zero, Cards.Zero, Cards.Zero, Cards.Zero, Cards.Zero, Cards.NorthWind
            }
        };

    }
}