using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.Roatp.Domain.Entities;

namespace SFA.DAS.Roatp.Domain.Interfaces
{
    public interface INationalAchievementRates_ImportReadRepository
    {
        Task<List<NationalAchievementRate_Import>> GetAllWithAchievementData();
    }
}