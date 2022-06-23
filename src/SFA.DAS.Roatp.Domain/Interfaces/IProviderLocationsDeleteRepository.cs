using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Domain.Interfaces
{
    public interface IProviderLocationsDeleteRepository
    {
        Task BulkDelete(IEnumerable<int> providerLocationIds);
    }
}
