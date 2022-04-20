using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Roatp.Api.Models;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Api.Services
{
    public class GetProviderCoursesService : IGetProviderCoursesService
    {
        private readonly IProviderCourseReadRepository _providerCourseReadRepository;
        private readonly IProviderReadRepository _providerReadRepository;
        private readonly ICourseReadRepository _courseReadRepository;

        public GetProviderCoursesService(IProviderCourseReadRepository providerCourseReadRepository, IProviderReadRepository providerReadRepository, ICourseReadRepository courseReadRepository)
        {
            _providerCourseReadRepository = providerCourseReadRepository;
            _providerReadRepository = providerReadRepository;
            _courseReadRepository = courseReadRepository;
        }

        public async Task<ProviderCourseModel> GetCourse(int ukprn, int larsCode)
        {
            var provider = await _providerReadRepository.GetByUkprn(ukprn);
            if (provider == null) return null;

            ProviderCourseModel providerCourse = await _providerCourseReadRepository.GetProviderCourse(provider.Id, larsCode);
            var courses = await _courseReadRepository.GetAllCourses();
            var course = courses?.FirstOrDefault(c => c.LarsCode == larsCode);
            providerCourse.IfateReferenceNumber = course?.IfateReferenceNumber;
            providerCourse.CourseName = course?.Title;
            providerCourse.Level = course != null ? course.Level : 0;

            return providerCourse;
        }

        public async Task<List<ProviderCourseModel>> GetAllCourses(int ukprn)
        {
            var provider = await _providerReadRepository.GetByUkprn(ukprn);
            if (provider == null) return new List<ProviderCourseModel>();

            var providerCourses = await _providerCourseReadRepository.GetAllProviderCourses(provider.Id);

            var providerCourseModels = providerCourses?.Select(p => (ProviderCourseModel)p).ToList();
            var courses = await _courseReadRepository.GetAllCourses();
            if(providerCourseModels != null)
            {
                foreach (var p in providerCourseModels)
                {
                    var course = courses?.FirstOrDefault(c => c.LarsCode == p.LarsCode);
                    p.IfateReferenceNumber = course?.IfateReferenceNumber;
                    p.CourseName = course?.Title;
                    p.Level = course != null ? course.Level : 0;
                }
            }
            return providerCourseModels;
        }
    }
}
