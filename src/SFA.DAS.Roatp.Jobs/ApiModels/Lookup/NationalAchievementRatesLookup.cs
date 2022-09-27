using SFA.DAS.Roatp.Domain.Models;
using System.Collections.Generic;

namespace SFA.DAS.Roatp.Jobs.ApiModels.Lookup
{
    public class NationalAchievementRatesLookup
    {
        public List<NationalAchievementRatesApiImport> NationalAchievementRates { get; set; } = new List<NationalAchievementRatesApiImport>();
        public List<NationalAchievementRatesOverallApiImport> OverallAchievementRates { get; set; } = new List<NationalAchievementRatesOverallApiImport>();
    }
}
