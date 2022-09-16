using SFA.DAS.Roatp.Domain.Entities;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Domain.Interfaces
{
    public interface IProviderLocationsWriteRepository
    {
        Task<ProviderLocation> Create(ProviderLocation location);
        Task UpdateProviderlocation(ProviderLocation updatedProviderLocationEntity);
    }
}
