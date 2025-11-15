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
    }

}