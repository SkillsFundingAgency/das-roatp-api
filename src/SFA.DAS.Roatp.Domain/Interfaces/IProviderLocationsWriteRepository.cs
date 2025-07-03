using SFA.DAS.Roatp.Domain.Entities;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Domain.Interfaces
{
    public interface IProviderLocationsWriteRepository
    {
        Task<ProviderLocation> Create(ProviderLocation location, int ukprn, string userId, string userDisplayName, string userAction);
        Task UpdateProviderlocation(ProviderLocation updatedProviderLocationEntity, int ukprn, string userId, string userDisplayName, string userAction);
        Task Delete(int ukprn, Guid id, string userId, string userDisplayName, string userAction);

    }
}
