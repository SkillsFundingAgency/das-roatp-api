using SFA.DAS.Roatp.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Jobs.Services
{
    public interface IReloadNationalAcheivementRatesService
    {
        Task ReloadNationalAcheivementRates(List<NationalAchievementRatesApiModel> nationalAchievementRatesImported);
    }
}