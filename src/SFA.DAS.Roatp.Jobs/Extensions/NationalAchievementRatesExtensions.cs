using SFA.DAS.Roatp.Domain.Models;

namespace SFA.DAS.Roatp.Jobs.Extensions;

public static class NationalAchievementRatesExtensions
{
    public static Age ToAgeEnum(this string value)
        => value?.Trim().ToLower() switch
        {
            "16-18" => Age.SixteenToEighteen,
            "19-23" => Age.NineteenToTwentyThree,
            "24+" => Age.TwentyFourPlus,
            "total" => Age.AllAges,
            _ => Age.Unknown
        };

    public static ApprenticeshipLevel ToApprenticeshipLevelEnum(this string value)
        => value?.Trim().ToLower() switch
        {
            "advanced level" => ApprenticeshipLevel.Three,
            "higher level" => ApprenticeshipLevel.FourPlus,
            "intermediate level" => ApprenticeshipLevel.Two,
            "total" => ApprenticeshipLevel.AllLevels,
            _ => ApprenticeshipLevel.Unknown
        };
}
