using SFA.DAS.Roatp.Domain.Entities;
using System.Collections.Generic;

namespace SFA.DAS.Roatp.Jobs.ApiModels.Lookup
{
    public class NationalAchievementRatesLookup
    {
        public List<NationalAchievementRate> NationalAchievementRates { get; set; } = new List<NationalAchievementRate>();
        public List<NationalAchievementRateOverall> OverallAchievementRates { get; set; } = new List<NationalAchievementRateOverall>();
    }
}
