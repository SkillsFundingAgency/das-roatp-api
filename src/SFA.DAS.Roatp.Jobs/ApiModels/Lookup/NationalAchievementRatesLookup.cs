using SFA.DAS.Roatp.Domain.Models;
using System.Collections.Generic;

namespace SFA.DAS.Roatp.Jobs.ApiModels.Lookup
{
    public class NationalAchievementRatesLookup
    {
        public List<NationalAchievementRatesApiModel> NationalAchievementRates { get; set; } = new List<NationalAchievementRatesApiModel>();
        public List<NationalAchievementRatesOverallApiModel> OverallAchievementRates { get; set; } = new List<NationalAchievementRatesOverallApiModel>();
    }
}
