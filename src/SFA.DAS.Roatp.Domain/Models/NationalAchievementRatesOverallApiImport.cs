namespace SFA.DAS.Roatp.Domain.Models
{
    public class NationalAchievementRatesOverallApiImport
    {
        public int Id { get; set; }
        public string Age { get; set; }
        public string SectorSubjectArea { get; set; }
        public string ApprenticeshipLevel { get; set; }
        public int? OverallCohort { get; set; }
        public decimal? OverallAchievementRate { get; set; }
    }
}