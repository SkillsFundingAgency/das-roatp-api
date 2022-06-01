using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.Roatp.Domain.Entities;

namespace SFA.DAS.Roatp.Domain.Interfaces
{
    public interface IProviderCourseReadRepository
    {
        Task<ProviderCourse> GetProviderCourse(int providerId, int larsCode);
        Task<ProviderCourse> GetProviderCourseByUkprn(int ukprn, int larsCode);
        Task<List<ProviderCourse>> GetAllProviderCourses(int providerId);
    }
}