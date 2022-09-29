namespace SFA.DAS.Roatp.Domain.Entities
{
    public class NationalAchievementRate 
    {
        public long Id { get; set; }
        public int Ukprn { get; set; }
        public int Age { get; set; }
        public string SectorSubjectArea { get; set; }
        public int ApprenticeshipLevel { get; set; }
        public int? OverallCohort { get; set; }
        public decimal? OverallAchievementRate { get; set; }
        public static implicit operator NationalAchievementRate(NationalAchievementRateImport source)
        {
            return new NationalAchievementRate
            {
                Age = source.Age,
                Ukprn = source.Ukprn,
                ApprenticeshipLevel = source.ApprenticeshipLevel,
                OverallCohort = source.OverallCohort,
                OverallAchievementRate = source.OverallAchievementRate,
                SectorSubjectArea = source.SectorSubjectArea
            };
        }
    }
}