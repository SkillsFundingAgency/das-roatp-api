using SFA.DAS.Roatp.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Jobs.Services
{
    public interface IReloadNationalAcheivementRatesOverallService
    {
        Task ReloadNationalAcheivementRatesOverall(List<NationalAchievementRatesOverallApiImport> OverallAchievementRatesImported);
    }
}