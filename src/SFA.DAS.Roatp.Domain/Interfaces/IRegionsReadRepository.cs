using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.Roatp.Domain.Entities;

namespace SFA.DAS.Roatp.Domain.Interfaces
{
    public interface IRegionsReadRepository
    {
        Task<List<Region>> GetAllRegions();
    }
}