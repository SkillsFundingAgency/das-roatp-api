using SFA.DAS.Roatp.Domain.Extensions;
using SFA.DAS.Roatp.Domain.Models;

namespace SFA.DAS.Roatp.Domain.Entities
{
    public class NationalAchievementRate_Import
    {
        public long Id { get; set; }
        public int Ukprn { get; set; }
        public int Age { get; set; }
        public string SectorSubjectArea { get; set; }
        public int ApprenticeshipLevel { get; set; }
        public int? OverallCohort { get; set; }
        public decimal? OverallAchievementRate { get; set; }
        public static implicit operator NationalAchievementRate_Import(NationalAchievementRatesApiImport source)
        {
            return new NationalAchievementRate_Import
            {
                Age = source.Age.ToAge(),
                Ukprn = source.Ukprn,
                ApprenticeshipLevel = source.ApprenticeshipLevel.ToApprenticeshipLevel(),
                OverallCohort = source.OverallCohort,
                OverallAchievementRate = source.OverallAchievementRate,
                SectorSubjectArea = source.SectorSubjectArea
            };
        }
    }
}