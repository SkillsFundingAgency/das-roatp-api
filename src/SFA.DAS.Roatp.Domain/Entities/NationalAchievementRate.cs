namespace SFA.DAS.Roatp.Domain.Entities
{
    public class NationalAchievementRate 
    {
        public int Id { get; set; }
        public int Ukprn { get; set; }
        public int Age { get; set; }
        public string SectorSubjectArea { get; set; }
        public int ApprenticeshipLevel { get; set; }
        public int? OverallCohort { get; set; }
        public decimal? OverallAchievementRate { get; set; }
    }
}