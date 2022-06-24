using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.Roatp.Domain.Entities;

namespace SFA.DAS.Roatp.Domain.Interfaces
{
    public interface IProviderLocationsReadRepository
    {
        Task<List<ProviderLocation>> GetAllProviderLocations(int ukprn);
        Task<List<ProviderLocation>> GetProviderLocationsById(int providerLocationId);
    }
}
