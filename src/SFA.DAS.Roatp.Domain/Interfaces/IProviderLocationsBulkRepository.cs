using SFA.DAS.Roatp.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Domain.Interfaces
{
    public interface IProviderLocationsBulkRepository
    {
        Task BulkInsert(IEnumerable<ProviderLocation> providerLocations);
        Task BulkDelete(IEnumerable<int> providerLocationIds, string userId, string userDisplayName, int ukprn, string userAction);
    }
}
