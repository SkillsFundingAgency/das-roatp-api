using System;
using SFA.DAS.Roatp.Domain.Models;

namespace SFA.DAS.Roatp.Domain.Entities
{
    public class NationalAchievementRateOverallImport
    {
        public long Id { get; set; }
        public Age Age { get; set; }
        public ApprenticeshipLevel ApprenticeshipLevel { get; set; }
        public int? OverallCohort { get; set; }
        public decimal? OverallAchievementRate { get; set; }
        public int SectorSubjectAreaTier1 { get; set; }

        [Obsolete]
        public static implicit operator NationalAchievementRateOverallImport(NationalAchievementRatesOverallApiModel source)
        {
            return new NationalAchievementRateOverallImport
            {
                Age = source.Age,
                ApprenticeshipLevel = source.ApprenticeshipLevel,
                OverallCohort = source.OverallCohort,
                OverallAchievementRate = source.OverallAchievementRate,
            };
        }
    }
}