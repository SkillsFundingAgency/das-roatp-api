using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.Roatp.Domain.Entities;

namespace SFA.DAS.Roatp.Domain.Interfaces
{
    public interface INationalAchievementRatesOverall_ImportWriteRepository
    {
        Task DeleteAll();
        Task InsertMany(List<NationalAchievementRateOverall_Import> items);
    }
}