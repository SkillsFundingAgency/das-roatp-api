using SFA.DAS.Roatp.Domain.Entities;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Domain.Interfaces
{
    public interface IProviderCoursesWriteRepository
    {
        Task<ProviderCourse> PatchProviderCourse(ProviderCourse patchedProviderCourseEntity, int ukprn, int larscode, string userId, string userDisplayName, string userAction);

        Task<ProviderCourse> CreateProviderCourse(ProviderCourse providerCourse);

        Task Delete(int ukprn, int larscode, string userId, string userDisplayName, string userAction);
    }
}
