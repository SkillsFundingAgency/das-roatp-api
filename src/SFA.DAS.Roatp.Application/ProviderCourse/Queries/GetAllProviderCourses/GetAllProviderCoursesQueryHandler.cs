using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.ProviderCourse.Queries.GetAllProviderCourses
{
    public class GetAllProviderCoursesQueryHandler : IRequestHandler<GetAllProviderCoursesQuery, GetAllProviderCoursesQueryResult>
    {
        private readonly IProviderCoursesReadRepository _providerCoursesReadRepository;
        private readonly IStandardsReadRepository _standardsReadRepository;
        private readonly ILogger<GetAllProviderCoursesQueryHandler> _logger;
        public GetAllProviderCoursesQueryHandler(IProviderCoursesReadRepository providerCoursesReadRepository, IStandardsReadRepository standardsReadRepository, ILogger<GetAllProviderCoursesQueryHandler> logger)
        {
            _providerCoursesReadRepository = providerCoursesReadRepository;
            _standardsReadRepository = standardsReadRepository;
            _logger = logger;
        }

        public async Task<GetAllProviderCoursesQueryResult> Handle(GetAllProviderCoursesQuery request, CancellationToken cancellationToken)
        {
            var providerCourses = await _providerCoursesReadRepository.GetAllProviderCourses(request.Ukprn);

            if (!providerCourses.Any())
            {
                _logger.LogInformation("ProviderCourses data not found for {ukprn}", request.Ukprn);
                return new GetAllProviderCoursesQueryResult { Courses = new List<ProviderCourseModel>() };
            }

            var providerCourseModels = providerCourses.Select(p => (ProviderCourseModel)p).ToList();
            var standardsLookup = await _standardsReadRepository.GetAllStandards();

            foreach (var p in providerCourseModels)
            {
                var course = standardsLookup.Single(c => c.LarsCode == p.LarsCode);
                p.AttachCourseDetails(course.IfateReferenceNumber, course.Level, course.Title, course.Version, course.ApprovalBody);
            }

            return new GetAllProviderCoursesQueryResult
            {
                Courses = providerCourseModels
            };
        }
    }
}
