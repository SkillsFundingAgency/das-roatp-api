using SFA.DAS.Roatp.Domain.Entities;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Domain.Interfaces
{
    public interface IProviderCourseLocationDeleteRepository
    {
        Task<int> BulkDelete(int ukprn, int larsCode, bool deleteProviderLocation);
    }
}
