using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Roatp.Api.Models;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Api.Services
{
    public class GetProviderCoursesService : IGetProviderCoursesService
    {
        private readonly IGetProviderCourseRepository _getProviderCourseRepository;

        public GetProviderCoursesService(IGetProviderCourseRepository getProviderCourseRepository)
        {
            _getProviderCourseRepository = getProviderCourseRepository;
        }

        public async Task<ProviderCourseModel> Get(int ukprn, int larsCode)
        {
            ProviderCourseModel providerCourse = await _getProviderCourseRepository.GetProviderCourse(ukprn, larsCode);
            return providerCourse;
        }

        public async Task<List<ProviderCourseModel>> GetAll(int ukprn)
        {
            var provider = await _getProviderCourseRepository.GetAllProviderCourse(ukprn);

            List<ProviderCourseModel> providerCourseModels = provider.Courses?.Select(p => (ProviderCourseModel)p).ToList();
            return providerCourseModels;
        }
    }
}
