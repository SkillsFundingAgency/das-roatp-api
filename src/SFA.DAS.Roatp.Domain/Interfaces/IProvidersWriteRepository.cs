using SFA.DAS.Roatp.Domain.Entities;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Domain.Interfaces
{
    public interface IProvidersWriteRepository
    {
        Task Create(Provider provider);
        Task Patch(Provider patchedProviderEntity, string userId, string userDisplayName, string userAction);
    }
}

