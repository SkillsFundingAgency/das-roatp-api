using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Models;

namespace SFA.DAS.Roatp.Api.ApiResponses
{
    public class GetOverallAchievementRateResponse
    {
        public string Age { get ; set ; }

        public string Level { get ; set ; }

        public string SectorSubjectArea { get ; set ; }

        public decimal? OverallAchievementRate { get ; set ; }

        public int? OverallCohort { get ; set ; }

        public static implicit operator GetOverallAchievementRateResponse(NationalAchievementRateOverall source)
        {
            return new GetOverallAchievementRateResponse
            {
                OverallCohort = source.OverallCohort,
                OverallAchievementRate = source.OverallAchievementRate,
                SectorSubjectArea = source.SectorSubjectArea,
                Age = ((Age)source.Age).ToString(),
                Level = ((ApprenticeshipLevel)source.ApprenticeshipLevel).ToString()
            };
        }
    }
}