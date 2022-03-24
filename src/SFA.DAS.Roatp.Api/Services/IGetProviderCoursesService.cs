using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.Roatp.Api.Models;

namespace SFA.DAS.Roatp.Api.Services
{
    public interface IGetProviderCoursesService
    {
        Task<List<ProviderCourseModel>> GetAllCourses(int ukprn);
        Task<ProviderCourseModel> GetCourse(int ukprn, int larsCode);
    }
}
