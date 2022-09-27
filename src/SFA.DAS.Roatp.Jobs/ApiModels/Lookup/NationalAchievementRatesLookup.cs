using System.Collections.Generic;

namespace SFA.DAS.Roatp.Jobs.ApiModels.Lookup
{
    public class NationalAchievementRatesLookup
    {
        public List<NationalAchievementRate_Import> NationalAchievementRates { get; set; } = new List<NationalAchievementRate_Import>();
        public List<NationalAchievementRateOverall_Import> OverallAchievementRates { get; set; } = new List<NationalAchievementRateOverall_Import>();
    }
}
