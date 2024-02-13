using SFA.DAS.Roatp.Domain.Models;

namespace SFA.DAS.Roatp.Domain.Entities
{
    public class NationalAchievementRateImport
    {
        public long Id { get; set; }
        public int Ukprn { get; set; }
        public Age Age { get; set; }
        public ApprenticeshipLevel ApprenticeshipLevel { get; set; }
        public int? OverallCohort { get; set; }
        public decimal? OverallAchievementRate { get; set; }
        public int SectorSubjectAreaTier1 { get; set; }
    }
}