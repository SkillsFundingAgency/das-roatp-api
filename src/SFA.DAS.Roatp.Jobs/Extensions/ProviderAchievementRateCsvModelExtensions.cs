using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Jobs.ApiModels.Lookup;
using SFA.DAS.Roatp.Jobs.Models;

namespace SFA.DAS.Roatp.Jobs.Extensions;
public static class ProviderAchievementRateCsvModelExtensions
{
    public static NationalAchievementRateImport ConvertToEntity(this ProviderAchievementRateCsvModel source, List<SectorSubjectAreaTier1Model> ssa1s)
        => new()
        {
            Ukprn = source.Ukprn,
            Age = source.AgeGroup.ToAgeEnum(),
            ApprenticeshipLevel = source.ApprenticeshipLevel.ToApprenticeshipLevelEnum(),
            OverallCohort = int.Parse(source.OverallCohort),
            OverallAchievementRate = decimal.Parse(source.OverallAchievementRate),
            SectorSubjectAreaTier1 = ssa1s.First(s => s.SectorSubjectAreaTier1Desc == source.SectorSubjectAreaTier1Desc).SectorSubjectAreaTier1
        };
}
