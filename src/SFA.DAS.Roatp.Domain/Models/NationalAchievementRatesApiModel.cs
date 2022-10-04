namespace SFA.DAS.Roatp.Domain.Models
{
    public class NationalAchievementRatesApiModel
    {
        public int Ukprn { get; set; }
        public Age Age { get; set; }
        public string SectorSubjectArea { get; set; }
        public ApprenticeshipLevel ApprenticeshipLevel { get; set; }
        public int? OverallCohort { get; set; }
        public decimal? OverallAchievementRate { get; set; }
    }
}