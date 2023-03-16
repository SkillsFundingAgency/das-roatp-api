using SFA.DAS.Roatp.Domain.Entities;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Domain.Interfaces
{
    public interface IProvidersWriteRepository
    {
        Task Patch(Provider patchedProviderEntity, string userId, string userDisplayName, string userAction);
        Task<Provider> Create(Provider provider, string userId, string userDisplayName, string userAction);
    }
}

