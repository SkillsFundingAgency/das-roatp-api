using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.Roatp.Domain.Models;

namespace SFA.DAS.Roatp.Domain.Interfaces
{
    public interface IProviderDetailsReadRepository
    {
        Task<ProviderCourseDetailsModel> GetProviderDetailsWithDistance(int ukprn, int larsCode, double? lat, double? lon);
        Task<List<ProviderCourseLocationDetailsModel>> GetProviderlocationDetailsWithDistance(int ukprn, int larsCode, double? lat, double? lon);

        Task<List<ProviderCourseDetailsSummaryModel>> GetAllProviderDetailsWithDistance(int larsCode, double? lat, double? lon);
        Task<List<ProviderCourseLocationDetailsModel>> GetAllProviderlocationDetailsWithDistance(int larsCode, double? lat, double? lon);


    }
}
