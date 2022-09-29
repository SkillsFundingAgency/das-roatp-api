using SFA.DAS.Roatp.Domain.Models;

namespace SFA.DAS.Roatp.Domain.Entities
{
    public class NationalAchievementRateOverallImport
    {
        public long Id { get; set; }
        public int Age { get; set; }
        public string SectorSubjectArea { get; set; }
        public int ApprenticeshipLevel { get; set; }
        public int? OverallCohort { get; set; }
        public decimal? OverallAchievementRate { get; set; }
        public static implicit operator NationalAchievementRateOverallImport(NationalAchievementRatesOverallApiImport source)
        {
            return new NationalAchievementRateOverallImport
            {
                Age = (int)source.Age,
                SectorSubjectArea = source.SectorSubjectArea,
                ApprenticeshipLevel = (int)source.ApprenticeshipLevel,
                OverallCohort = source.OverallCohort,
                OverallAchievementRate = source.OverallAchievementRate,
            };
        }
    }
}