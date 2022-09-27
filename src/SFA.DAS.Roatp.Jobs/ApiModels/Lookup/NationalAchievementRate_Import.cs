namespace SFA.DAS.Roatp.Jobs.ApiModels.Lookup
{
    public class NationalAchievementRate_Import
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