using SFA.DAS.Roatp.Domain.Entities;


namespace SFA.DAS.Roatp.Application.OverallNationalAchievementRates.Queries.GetOverallAchievementRates
{
    public class GetOverallAchievementRateResponse
    {
        public string Age { get; set; }

        public string Level { get; set; }

        public string SectorSubjectArea { get; set; }

        public decimal? OverallAchievementRate { get; set; }

        public int? OverallCohort { get; set; }

        public static implicit operator GetOverallAchievementRateResponse(NationalAchievementRateOverall source)
        {
            return new GetOverallAchievementRateResponse
            {
                OverallCohort = source.OverallCohort,
                OverallAchievementRate = source.OverallAchievementRate,
                SectorSubjectArea = source.SectorSubjectArea,
                Age = source.Age.ToString(),
                Level = source.ApprenticeshipLevel.ToString()
            };
        }
    }
}