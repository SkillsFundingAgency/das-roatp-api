using SFA.DAS.Roatp.Domain.Extensions;
using SFA.DAS.Roatp.Domain.Models;

namespace SFA.DAS.Roatp.Domain.Entities
{
    public class NationalAchievementRateOverall_Import
    {
        public long Id { get; set; }
        public int Age { get; set; }
        public string SectorSubjectArea { get; set; }
        public int ApprenticeshipLevel { get; set; }
        public int? OverallCohort { get; set; }
        public decimal? OverallAchievementRate { get; set; }
        public static implicit operator NationalAchievementRateOverall_Import(NationalAchievementRatesOverallApiImport source)
        {
            return new NationalAchievementRateOverall_Import
            {
                Age = source.Age.ToAge(),
                SectorSubjectArea = source.SectorSubjectArea,
                ApprenticeshipLevel = source.ApprenticeshipLevel.ToApprenticeshipLevel(),
                OverallCohort = source.OverallCohort,
                OverallAchievementRate = source.OverallAchievementRate,
            };
        }
    }
}