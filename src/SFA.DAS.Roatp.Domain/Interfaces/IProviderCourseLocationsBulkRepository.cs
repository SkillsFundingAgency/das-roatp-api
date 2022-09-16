using SFA.DAS.Roatp.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Domain.Interfaces
{
    public interface IProviderCourseLocationsBulkRepository
    {
        Task BulkInsert(IEnumerable<ProviderCourseLocation> providerCourseLocations);
        Task BulkDelete(IEnumerable<int> providerCourseLocationIds);
    }
}
