using SFA.DAS.Roatp.Domain.Models;

namespace SFA.DAS.Roatp.Domain.Entities
{
    public class NationalAchievementRate 
    {
        public long Id { get; set; }
        public int ProviderId { get; set; }
        public Age Age { get; set; }
        public string SectorSubjectArea { get; set; }
        public ApprenticeshipLevel ApprenticeshipLevel { get; set; }
        public int? OverallCohort { get; set; }
        public decimal? OverallAchievementRate { get; set; }
        public virtual Provider Provider { get; set; }
        public static implicit operator NationalAchievementRate(NationalAchievementRateImport source)
        {
            return new NationalAchievementRate
            {
                Age = source.Age,
                ApprenticeshipLevel = source.ApprenticeshipLevel,
                OverallCohort = source.OverallCohort,
                OverallAchievementRate = source.OverallAchievementRate,
                SectorSubjectArea = source.SectorSubjectArea
            };
        }
    }
}