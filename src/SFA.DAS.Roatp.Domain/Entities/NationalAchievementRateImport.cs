using SFA.DAS.Roatp.Domain.Models;

namespace SFA.DAS.Roatp.Domain.Entities
{
    public class NationalAchievementRateImport
    {
        public long Id { get; set; }
        public int Ukprn { get; set; }
        public int Age { get; set; }
        public string SectorSubjectArea { get; set; }
        public int ApprenticeshipLevel { get; set; }
        public int? OverallCohort { get; set; }
        public decimal? OverallAchievementRate { get; set; }
        public static implicit operator NationalAchievementRateImport(NationalAchievementRatesApiImport source)
        {
            return new NationalAchievementRateImport
            {
                Age = (int)source.Age,
                Ukprn = source.Ukprn,
                ApprenticeshipLevel = (int)source.ApprenticeshipLevel,
                OverallCohort = source.OverallCohort,
                OverallAchievementRate = source.OverallAchievementRate,
                SectorSubjectArea = source.SectorSubjectArea
            };
        }
    }
}