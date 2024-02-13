using SFA.DAS.Roatp.Domain.Models;

namespace SFA.DAS.Roatp.Domain.Entities
{
    public class NationalAchievementRateOverall
    {
        public long Id { get; set; }
        public Age Age { get; set; }
        public ApprenticeshipLevel ApprenticeshipLevel { get; set; }
        public int? OverallCohort { get; set; }
        public decimal? OverallAchievementRate { get; set; }
        public int SectorSubjectAreaTier1 { get; set; }

        public static implicit operator NationalAchievementRateOverall(NationalAchievementRateOverallImport source)
        {
            return new NationalAchievementRateOverall
            {
                Age = source.Age,
                ApprenticeshipLevel = source.ApprenticeshipLevel,
                OverallCohort = source.OverallCohort,
                OverallAchievementRate = source.OverallAchievementRate,
                SectorSubjectAreaTier1 = source.SectorSubjectAreaTier1
            };
        }
    }
}