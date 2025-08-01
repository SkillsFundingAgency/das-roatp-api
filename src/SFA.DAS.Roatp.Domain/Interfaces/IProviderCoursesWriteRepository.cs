using System.Threading.Tasks;
using SFA.DAS.Roatp.Domain.Entities;

namespace SFA.DAS.Roatp.Domain.Interfaces
{
    public interface IProviderCoursesWriteRepository
    {
        Task<ProviderCourse> PatchProviderCourse(ProviderCourse patchedProviderCourseEntity, int ukprn, int larscode, string userId, string userDisplayName, string userAction);

        Task<ProviderCourse> CreateProviderCourse(ProviderCourse providerCourse, int ukprn, string userId, string userDisplayName, string userAction);

        Task Delete(int ukprn, int larscode, string userId, string userDisplayName, string userAction);
    }
}
