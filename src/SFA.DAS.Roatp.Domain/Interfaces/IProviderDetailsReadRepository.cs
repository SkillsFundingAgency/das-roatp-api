
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.Roatp.Domain.Entities;

namespace SFA.DAS.Roatp.Domain.Interfaces
{
    public interface IProviderDetailsReadRepository
    {
        Task<ProviderAndCourseDetailsWithDistance> GetProviderDetailsWithDistance(int ukprn, int larsCode, double? lat, double? lon);
        Task<List<ProviderLocationDetailsWithDistance>> GetProviderlocationDetailsWithDistance(int ukprn, int larsCode, double? lat, double? lon);
    }
}
