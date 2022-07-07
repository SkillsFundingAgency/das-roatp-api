using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Domain.Interfaces
{
    public interface IProviderCourseLocationDeleteRepository
    {
        Task BulkDelete(IEnumerable<int> providerCourseLocationIds);
        Task Delete(int providerCourseLocationId);
    }
}
