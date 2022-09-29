using SFA.DAS.Roatp.Domain.Entities;
using System.Collections.Generic;

namespace SFA.DAS.Roatp.Application.OverallNationalAchievementRates.Queries
{
    public class GetOverallAchievementRatesResponse
    {
        public List<NationalAchievementRateOverall> OverallAchievementRates { get ; set ; }
    }
}