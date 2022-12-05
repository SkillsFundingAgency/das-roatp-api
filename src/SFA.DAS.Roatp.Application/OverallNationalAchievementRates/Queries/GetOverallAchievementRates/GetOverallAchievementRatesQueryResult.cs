using SFA.DAS.Roatp.Domain.Entities;
using System.Collections.Generic;

namespace SFA.DAS.Roatp.Application.OverallNationalAchievementRates.Queries.GetOverallAchievementRates
{
    public class GetOverallAchievementRatesQueryResult
    {
        public List<NationalAchievementRateOverallModel> OverallAchievementRates { get; set; }
    }
}