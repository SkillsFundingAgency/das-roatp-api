using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Api.Models;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Api.Services
{
    public class GetProviderCoursesService : IGetProviderCoursesService
    {
        private readonly IProviderCourseReadRepository _providerCourseReadRepository;
        private readonly IProviderReadRepository _providerReadRepository;
        private readonly IStandardReadRepository _courseReadRepository;
        private readonly ILogger<GetProviderCoursesService> _logger;

        public GetProviderCoursesService(IProviderCourseReadRepository providerCourseReadRepository, IProviderReadRepository providerReadRepository, IStandardReadRepository courseReadRepository, ILogger<GetProviderCoursesService> logger)
        {
            _providerCourseReadRepository = providerCourseReadRepository;
            _providerReadRepository = providerReadRepository;
            _courseReadRepository = courseReadRepository;
            _logger = logger;
        }

        public async Task<ProviderCourseModel> GetCourse(int ukprn, int larsCode)
        {
            var provider = await _providerReadRepository.GetByUkprn(ukprn);
            if (provider == null)
            {
                _logger.LogInformation("Provider data not found for {ukprn} and {larsCode}", ukprn, larsCode);
                return null;
            }

            ProviderCourseModel providerCourse = await _providerCourseReadRepository.GetProviderCourse(provider.Id, larsCode);
            var courseLookup = await _courseReadRepository.GetCourse(larsCode);
            if (courseLookup == null)
            {
                _logger.LogError("Courses Lookup data not found for {ukprn} and {larsCode}", ukprn, larsCode);
                return null;
            }
            providerCourse.UpdateCourseDetails(courseLookup.IfateReferenceNumber, courseLookup.Level, courseLookup.Title);

            return providerCourse;
        }

        public async Task<List<ProviderCourseModel>> GetAllCourses(int ukprn)
        {
            var provider = await _providerReadRepository.GetByUkprn(ukprn);
            if (provider == null)
            {
                _logger.LogInformation("Provider data not found for {ukprn}", ukprn);
                return new List<ProviderCourseModel>();
            }

            var providerCourses = await _providerCourseReadRepository.GetAllProviderCourses(provider.Id);
            if (!providerCourses.Any())
            {
                _logger.LogInformation("ProviderCourses data not found for {ukprn}", ukprn);
                return new List<ProviderCourseModel>();
            }

            var providerCourseModels = providerCourses.Select(p => (ProviderCourseModel)p).ToList();
            var coursesLookup = await _courseReadRepository.GetAllCourses();
            if (!coursesLookup.Any())
            {
                _logger.LogError("Courses Lookup data not found for {ukprn}", ukprn);
                throw new InvalidOperationException($"Courses Lookup data not found for {ukprn}");
            }
            foreach (var p in providerCourseModels)
            {
                var course = coursesLookup.Single(c => c.LarsCode == p.LarsCode);
                p.UpdateCourseDetails(course.IfateReferenceNumber, course.Level, course.Title);
            }
            return providerCourseModels;
        }
    }
}
