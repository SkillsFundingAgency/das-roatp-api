using SFA.DAS.Roatp.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Domain.Interfaces
{
    public interface IProviderLocationsInsertRepository
    {
        Task BulkInsert(IEnumerable<ProviderLocation> providerLocations);
        Task Create(ProviderLocation providerLocation);
    }
}
