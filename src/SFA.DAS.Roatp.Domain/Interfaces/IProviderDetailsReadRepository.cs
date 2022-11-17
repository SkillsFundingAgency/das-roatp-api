using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.Roatp.Domain.Models;

namespace SFA.DAS.Roatp.Domain.Interfaces
{
    public interface IProviderDetailsReadRepository
    {
        Task<ProviderCourseDetailsModel> GetProviderForUkprnAndLarsCodeWithDistance(int ukprn, int larsCode, decimal? lat, decimal? lon);
        Task<List<ProviderCourseLocationDetailsModel>> GetProviderLocationDetailsWithDistance(int ukprn, int larsCode, decimal? lat, decimal? lon);

        Task<List<ProviderCourseDetailsSummaryModel>> GetProvidersForLarsCodeWithDistance(int larsCode, decimal? lat, decimal? lon);
        Task<List<ProviderCourseLocationDetailsModel>> GetAllProviderlocationDetailsWithDistance(int larsCode, decimal? lat, decimal? lon);


    }
}
