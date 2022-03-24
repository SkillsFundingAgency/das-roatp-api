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

        public GetProviderCoursesService(IProviderCourseReadRepository providerCourseReadRepository, IProviderReadRepository providerReadRepository)
        {
            _providerCourseReadRepository = providerCourseReadRepository;
            _providerReadRepository = providerReadRepository;
        }

        public async Task<ProviderCourseModel> GetCourse(int ukprn, int larsCode)
        {
            var provider = await _providerReadRepository.GetByUkprn(ukprn);
            if (provider == null) return null;

            ProviderCourseModel providerCourse = await _providerCourseReadRepository.GetProviderCourse(provider.Id, larsCode);
            return providerCourse;
        }

        public async Task<List<ProviderCourseModel>> GetAllCourses(int ukprn)
        {
            var provider = await _providerReadRepository.GetByUkprn(ukprn);
            if (provider == null) return new List<ProviderCourseModel>();

            var courses = await _providerCourseReadRepository.GetAllProviderCourses(provider.Id);

            var providerCourseModels = courses?.Select(p => (ProviderCourseModel)p).ToList();

            return providerCourseModels;
        }
    }
}
