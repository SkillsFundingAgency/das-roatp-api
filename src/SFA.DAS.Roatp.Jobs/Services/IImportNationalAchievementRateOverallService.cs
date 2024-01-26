using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.Roatp.Jobs.Models;

namespace SFA.DAS.Roatp.Jobs.Services;

public interface IImportNationalAchievementRateOverallService
{
    Task ImportData(IEnumerable<OverallAchievementRateCsvModel> rawData, List<ApiModels.Lookup.SectorSubjectAreaTier1Model> ssa1s);
}
