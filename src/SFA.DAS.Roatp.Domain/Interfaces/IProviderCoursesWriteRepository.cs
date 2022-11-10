using SFA.DAS.Roatp.Domain.Entities;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Domain.Interfaces
{
    public interface IProviderCoursesWriteRepository
    {
        Task<ProviderCourse> PatchProviderCourse(ProviderCourse patchedProviderCourseEntity);

        Task<ProviderCourse> CreateProviderCourse(ProviderCourse providerCourse);

        Task Delete(int ukprn, int larscode, string UserId, string CorrelationId);
    }
}
