using Mahjong.Core.Util;

namespace Mahjong.Resolution;

public partial class SampleBoards
{
    public static Cards[,] EaseBoard1 = new Cards[8, 6]
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
             Cards.SixRank, Cards.WestWind, Cards.ThreeDots, Cards.SixRank, Cards.ThreeDots, Cards.WhiteDrg,
        },
        {
             Cards.OneBmb, Cards.SevenBmb, Cards.ThreeDots, Cards.WestWind, Cards.TwoBmb, Cards.EastWind,
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

    public static Cards[,] ExpertBoard3 = new Cards[12, 10]
    {
        {
            Cards.WestWind, Cards.NorthWind, Cards.Bamboo, Cards.FiveDots, Cards.EastWind, Cards.SixDots, Cards.SixDots, Cards.ThreeBmb, Cards.FiveBmb, Cards.EightRank
        },
        {
            Cards.WestWind, Cards.NorthWind, Cards.FiveRank, Cards.OneDots, Cards.NorthWind, Cards.ThreeDots, Cards.Autumn, Cards.SixDots, Cards.EightDots, Cards.OneDots
        },
        {
            Cards.NineDots, Cards.ThreeBmb, Cards.ThreeDots, Cards.SixBmb, Cards.OneBmb, Cards.FiveBmb, Cards.SixDots, Cards.TwoBmb, Cards.Summer, Cards.NineBmb
        },
        {
            Cards.Summer, Cards.ThreeBmb, Cards.GreenDrg, Cards.NineRank, Cards.OneBmb, Cards.GreenDrg, Cards.FourDots, Cards.Chrysan, Cards.EightDots, Cards.Autumn
        },
        {
            Cards.ThreeRank, Cards.OneRank, Cards.FourDots, Cards.OneRank, Cards.NineRank, Cards.TwoBmb, Cards.OneDots, Cards.NineDots, Cards.GreenDrg, Cards.NorthWind
        },
        {
            Cards.NorthWind, Cards.SixBmb, Cards.OneBmb, Cards.FourDots, Cards.FiveDots, Cards.Autumn, Cards.SevenDots, Cards.NorthWind, Cards.Autumn, Cards.TwoBmb
        },
        {
            Cards.FiveDots, Cards.Chrysan, Cards.EightRank, Cards.FiveRank, Cards.Summer, Cards.SevenDots, Cards.TwoRank, Cards.OneDots, Cards.NineRank, Cards.NineDots
        },
        {
            Cards.ThreeBmb, Cards.TwoRank, Cards.NineBmb, Cards.OneRank, Cards.ThreeRank, Cards.EightDots, Cards.EastWind, Cards.OneBmb, Cards.Summer, Cards.NorthWind
        },
        {
            Cards.EightDots, Cards.Chrysan, Cards.TwoBmb, Cards.WestWind, Cards.NineBmb, Cards.SevenBmb, Cards.SixBmb, Cards.NineDots, Cards.FiveDots, Cards.GreenDrg
        },
        {
            Cards.Chrysan, Cards.EightRank, Cards.NineRank, Cards.FiveBmb, Cards.Bamboo, Cards.Bamboo, Cards.FiveBmb, Cards.FourDots, Cards.FiveRank, Cards.SevenBmb
        },
        {
            Cards.ThreeDots, Cards.OneRank, Cards.SevenBmb, Cards.SevenBmb, Cards.SixBmb, Cards.SevenDots, Cards.Bamboo, Cards.ThreeRank, Cards.EastWind, Cards.TwoRank
        },
        {
            Cards.ThreeRank, Cards.EightRank, Cards.TwoRank, Cards.ThreeDots, Cards.WestWind, Cards.NorthWind, Cards.NineBmb, Cards.FiveRank, Cards.SevenDots, Cards.EastWind
        },
    };


    public static Cards[,] ExpertBoard4 = new Cards[12, 10]
    {
        {
            Cards.OneRank, Cards.SixBmb, Cards.FourBmb, Cards.Summer, Cards.EightDots, Cards.FourDots, Cards.SevenDots,
            Cards.SevenBmb, Cards.EightBmb, Cards.SevenDots,
        },
        {
            Cards.Summer, Cards.FiveDots, Cards.Orchid, Cards.ThreeDots, Cards.SixDots, Cards.FourDots, Cards.NorthWind,
            Cards.WestWind, Cards.FiveBmb, Cards.EightDots,
        },
        {
            Cards.FiveBmb, Cards.FiveBmb, Cards.SouthWind, Cards.SevenRank, Cards.NineBmb, Cards.WhiteDrg,
            Cards.FiveRank, Cards.EastWind, Cards.Chrysan, Cards.SixDots,
        },
        {
            Cards.FourRank, Cards.GreenDrg, Cards.ThreeRank, Cards.OneRank, Cards.FiveBmb, Cards.WestWind,
            Cards.FiveRank, Cards.Orchid, Cards.SouthWind, Cards.SevenRank,
        },
        {
            Cards.SevenRank, Cards.OneDots, Cards.ThreeRank, Cards.WhiteDrg, Cards.EastWind, Cards.OneRank,
            Cards.OneDots, Cards.FourRank, Cards.WestWind, Cards.FiveDots,
        },
        {
            Cards.ThreeRank, Cards.ThreeDots, Cards.SevenBmb, Cards.GreenDrg, Cards.OneDots, Cards.Chrysan,
            Cards.FourRank, Cards.Autumn, Cards.SouthWind, Cards.Orchid,
        },
        {
            Cards.SixDots, Cards.FiveRank, Cards.ThreeDots, Cards.NineBmb, Cards.NineRank, Cards.TwoDots,
            Cards.EightDots, Cards.GreenDrg, Cards.FourBmb, Cards.NineBmb,
        },
        {
            Cards.Chrysan, Cards.ThreeRank, Cards.OneRank, Cards.FiveDots, Cards.FourRank, Cards.SevenRank,
            Cards.EightBmb, Cards.NorthWind, Cards.SixBmb, Cards.NineRank,
        },
        {
            Cards.EightBmb, Cards.SixDots, Cards.OneDots, Cards.EastWind, Cards.WestWind, Cards.FourBmb,
            Cards.Autumn, Cards.NorthWind, Cards.NineRank, Cards.TwoDots,
        },
        {
            Cards.SevenBmb, Cards.Autumn, Cards.EightBmb, Cards.SevenBmb, Cards.SevenDots, Cards.FourBmb,
            Cards.EastWind, Cards.GreenDrg, Cards.ThreeDots, Cards.NorthWind,
        },
        {
            Cards.SevenDots, Cards.Autumn, Cards.WhiteDrg, Cards.Summer, Cards.Orchid, Cards.SixBmb,
            Cards.TwoDots, Cards.SixBmb, Cards.FourDots, Cards.FiveDots,
        },
        {
            Cards.WhiteDrg, Cards.Chrysan, Cards.NineRank, Cards.Summer, Cards.TwoDots, Cards.FiveRank,
            Cards.SouthWind, Cards.NineBmb, Cards.FourDots, Cards.EightDots,
        }
    };

}