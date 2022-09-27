namespace SFA.DAS.Roatp.Domain.Entities
{
    public class NationalAchievementRateOverall 
    {
        public long Id { get; set; }
        public int Age { get; set; }
        public string SectorSubjectArea { get; set; }
        public int ApprenticeshipLevel { get; set; }
        public int? OverallCohort { get; set; }
        public decimal? OverallAchievementRate { get; set; }
        public static implicit operator NationalAchievementRateOverall(NationalAchievementRateOverall_Import source)
        {
            return new NationalAchievementRateOverall
            {
                Age = source.Age,
                SectorSubjectArea = source.SectorSubjectArea,
                ApprenticeshipLevel = source.ApprenticeshipLevel,
                OverallCohort = source.OverallCohort,
                OverallAchievementRate = source.OverallAchievementRate,
            };
        }
    }
}