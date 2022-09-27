
using SFA.DAS.Roatp.Domain.Models;

namespace SFA.DAS.Roatp.Domain.Extensions
{
    public static class EnumExtensions
    {
        public static int ToAge(this string value)
        {
            switch (value)
            {
                case "SixteenToEighteen":
                    return (int)Age.SixteenToEighteen;
                case "NineteenToTwentyThree":
                    return (int)Age.NineteenToTwentyThree;
                case "TwentyFourPlus":
                    return (int)Age.TwentyFourPlus;
                case "AllAges":
                    return (int)Age.AllAges;
                default:
                    return (int)Age.Unknown;
            }
        }

        public static int ToApprenticeshipLevel(this string value)
        {
            switch (value)
            {
                case "Two":
                    return (int)ApprenticeshipLevel.Two;
                case "Three":
                    return (int)ApprenticeshipLevel.Three;
                case "FourPlus":
                    return (int)ApprenticeshipLevel.FourPlus;
                case "AllLevels":
                    return (int)ApprenticeshipLevel.AllLevels;
                default:
                    return (int)ApprenticeshipLevel.Unknown;
            }
        }
    }
}