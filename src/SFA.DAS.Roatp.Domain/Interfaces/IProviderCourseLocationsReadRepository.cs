using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.Roatp.Domain.Entities;

namespace SFA.DAS.Roatp.Domain.Interfaces
{
    public interface IProviderCourseLocationsReadRepository
    {
        Task<List<ProviderCourseLocation>> GetAllProviderCourseLocations(int ukprn, string larsCode);
        Task<List<ProviderCourseLocation>> GetProviderCourseLocationsByUkprn(int ukprn);
    }
}