using SFA.DAS.Roatp.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Domain.Interfaces
{
    public interface IProviderCoursesReadRepository
    {
        Task<ProviderCourse> GetProviderCourse(int providerId, int larsCode);
        Task<ProviderCourse> GetProviderCourseByUkprn(int ukprn, int larsCode);
        Task<List<ProviderCourse>> GetAllProviderCourses(int ukprn);
    }
}