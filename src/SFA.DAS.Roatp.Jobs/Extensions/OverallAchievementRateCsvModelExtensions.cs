using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Jobs.ApiModels.Lookup;
using SFA.DAS.Roatp.Jobs.Models;

namespace SFA.DAS.Roatp.Jobs.Extensions;

public static class OverallAchievementRateCsvModelExtensions
{
    public static NationalAchievementRateOverallImport ConvertToEntity(this OverallAchievementRateCsvModel source, List<SectorSubjectAreaTier1Model> ssa1s)
        => new()
        {
            Age = source.AgeGroup.ToAgeEnum(),
            ApprenticeshipLevel = source.ApprenticeshipLevel.ToApprenticeshipLevelEnum(),
            OverallCohort = int.Parse(source.OverallCohort),
            OverallAchievementRate = decimal.Parse(source.OverallAchievementRate),
            SectorSubjectAreaTier1 = ssa1s.FirstOrDefault(s => s.SectorSubjectAreaTier1Desc == source.SectorSubjectAreaTier1Desc).SectorSubjectAreaTier1
        };
}
