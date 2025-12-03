using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.Roatp.Domain.Entities;

namespace SFA.DAS.Roatp.Domain.Interfaces
{
    public interface IProviderCoursesReadRepository
    {
        Task<ProviderCourse> GetProviderCourse(int providerId, string larsCode);
        Task<ProviderCourse> GetProviderCourseByUkprn(int ukprn, string larsCode);
        Task<List<ProviderCourse>> GetAllProviderCourses(int ukprn);
    }
}