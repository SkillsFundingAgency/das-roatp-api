using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.Roatp.Domain.Entities;

namespace SFA.DAS.Roatp.Domain.Interfaces
{
    public interface INationalAchievementRatesOverall_ImportReadRepository
    {
        Task<List<NationalAchievementRateOverall_Import>> GetAllWithAchievementData();
    }
}