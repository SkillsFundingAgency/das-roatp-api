using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Domain.Interfaces
{
    public interface IProviderCourseLocationsDeleteRepository
    {
        Task BulkDelete(IEnumerable<int> providerCourseLocationIds);
    }
}
