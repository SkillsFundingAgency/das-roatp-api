using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Models;

namespace SFA.DAS.Roatp.Application.OverallNationalAchievementRates.Queries.GetOverallAchievementRates;

public class NationalAchievementRateOverallModel
{
    public Age Age { get; set; }
    public ApprenticeshipLevel Level { get; set; }
    public int? OverallCohort { get; set; }
    public decimal? OverallAchievementRate { get; set; }
    public static implicit operator NationalAchievementRateOverallModel(NationalAchievementRateOverall source)
    {
        return new NationalAchievementRateOverallModel
        {
            Age = source.Age,
            Level = source.ApprenticeshipLevel,
            OverallCohort = source.OverallCohort,
            OverallAchievementRate = source.OverallAchievementRate,
        };
    }
}