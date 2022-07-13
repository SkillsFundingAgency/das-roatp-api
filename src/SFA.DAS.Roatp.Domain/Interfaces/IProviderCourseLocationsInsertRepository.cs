using SFA.DAS.Roatp.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Domain.Interfaces
{
    public interface IProviderCourseLocationsInsertRepository
    {
        Task BulkInsert(IEnumerable<ProviderCourseLocation> providerCourseLocations);
    }
}
