using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Application.Locations.Queries;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.ProviderCourse.Queries
{
    public class ProviderAllCoursesQueryHandler : IRequestHandler<ProviderAllCoursesQuery, ProviderAllCoursesQueryResult>
    {
        private readonly IProviderCourseReadRepository _providerCourseReadRepository;
        private readonly IProviderReadRepository _providerReadRepository;
        private readonly IStandardReadRepository _standardReadRepository;
        private readonly ILogger<ProviderAllCoursesQueryHandler> _logger;
        public ProviderAllCoursesQueryHandler(IProviderCourseReadRepository providerCourseReadRepository, IProviderReadRepository providerReadRepository, IStandardReadRepository standardReadRepository, ILogger<ProviderAllCoursesQueryHandler> logger)
        {
            _providerCourseReadRepository = providerCourseReadRepository;
            _providerReadRepository = providerReadRepository;
            _standardReadRepository = standardReadRepository;
            _logger = logger;
        }

        public async  Task<ProviderAllCoursesQueryResult> Handle(ProviderAllCoursesQuery request, CancellationToken cancellationToken)
        {
            var ukprn = request.Ukprn;

            var provider = await _providerReadRepository.GetByUkprn(ukprn);
            if (provider == null)
            {
                _logger.LogInformation("Provider data not found for {ukprn}", ukprn);
                return new ProviderAllCoursesQueryResult { Courses = new List<ProviderCourseModel>() };
            }

            var providerCourses = await _providerCourseReadRepository.GetAllProviderCourses(provider.Id);
            if (!providerCourses.Any())
            {
                _logger.LogInformation("ProviderCourses data not found for {ukprn}", ukprn);
                return new ProviderAllCoursesQueryResult { Courses = new List<ProviderCourseModel>() };
            }

            var providerCourseModels = providerCourses.Select(p => (ProviderCourseModel)p).ToList();
            if (!providerCourseModels.Any())
            {
                _logger.LogError("Provider courses data not found for {ukprn}", ukprn);
                throw new InvalidOperationException($"Provider courses not found for {ukprn}");
            }

            var standardsLookup = await _standardReadRepository.GetAllStandards();
            if (!standardsLookup.Any())
            {
                _logger.LogError("Standards Lookup data not found for {ukprn}", ukprn);
                throw new InvalidOperationException($"Standards Lookup data not found for {ukprn}");
            }

            foreach (var p in providerCourseModels)
            {
                var course = standardsLookup.Single(c => c.LarsCode == p.LarsCode);
                p.UpdateCourseDetails(course.IfateReferenceNumber, course.Level, course.Title, course.Version, course.ApprovalBody);
            }

            return new ProviderAllCoursesQueryResult
            {
                Courses = providerCourseModels
            };
        }
    }
}
