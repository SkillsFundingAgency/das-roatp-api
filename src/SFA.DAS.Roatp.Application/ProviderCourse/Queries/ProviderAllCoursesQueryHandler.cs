using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation.Validators;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Application.Locations.Queries;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.ProviderCourse.Queries
{
    public class ProviderAllCoursesQueryHandler : IRequestHandler<ProviderAllCoursesQuery, ProviderAllCoursesQueryResult>
    {
        private readonly IProviderCourseReadRepository _providerCourseReadRepository;
        private readonly IStandardReadRepository _standardReadRepository;
        private readonly ILogger<ProviderAllCoursesQueryHandler> _logger;
        public ProviderAllCoursesQueryHandler(IProviderCourseReadRepository providerCourseReadRepository, IStandardReadRepository standardReadRepository, ILogger<ProviderAllCoursesQueryHandler> logger)
        {
            _providerCourseReadRepository = providerCourseReadRepository;
            _standardReadRepository = standardReadRepository;
            _logger = logger;
        }

        public async  Task<ProviderAllCoursesQueryResult> Handle(ProviderAllCoursesQuery request, CancellationToken cancellationToken)
        {
            var providerCourses = await _providerCourseReadRepository.GetAllProviderCourses(request.Ukprn);

            
            if (!providerCourses.Any())
            {
                _logger.LogInformation("ProviderCourses data not found for {ukprn}", request.Ukprn);
                return new ProviderAllCoursesQueryResult { Courses = new List<ProviderCourseModel>() };
            }

            var providerCourseModels = providerCourses.Select(p => (ProviderCourseModel)p).ToList();
            var standardsLookup = await _standardReadRepository.GetAllStandards();
            
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
