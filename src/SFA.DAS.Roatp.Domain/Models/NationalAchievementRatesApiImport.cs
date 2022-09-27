namespace SFA.DAS.Roatp.Domain.Models
{
    public class NationalAchievementRatesApiImport
    {
        public int Id { get; set; }
        public int Ukprn { get; set; }
        public string Age { get; set; }
        public string SectorSubjectArea { get; set; }
        public string ApprenticeshipLevel { get; set; }
        public int? OverallCohort { get; set; }
        public decimal? OverallAchievementRate { get; set; }
    }
}