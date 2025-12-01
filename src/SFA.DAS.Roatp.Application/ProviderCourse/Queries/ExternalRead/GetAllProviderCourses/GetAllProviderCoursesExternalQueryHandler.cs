using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Application.Mediatr.Responses;
using SFA.DAS.Roatp.Application.ProviderCourse.Queries.ExternalRead.GetProviderCourse;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.ProviderCourse.Queries.ExternalRead.GetAllProviderCourses
{
    public class GetAllProviderCoursesExternalQueryHandler : IRequestHandler<GetAllProviderCoursesExternalQuery, ValidatedResponse<List<ProviderCourseModelExternal>>>
    {
        private readonly IProviderCoursesReadRepository _providerCoursesReadRepository;
        private readonly IStandardsReadRepository _standardsReadRepository;
        private readonly ILogger<GetAllProviderCoursesExternalQueryHandler> _logger;
        public GetAllProviderCoursesExternalQueryHandler(IProviderCoursesReadRepository providerCoursesReadRepository, IStandardsReadRepository standardsReadRepository, ILogger<GetAllProviderCoursesExternalQueryHandler> logger)
        {
            _providerCoursesReadRepository = providerCoursesReadRepository;
            _standardsReadRepository = standardsReadRepository;
            _logger = logger;
        }

        public async Task<ValidatedResponse<List<ProviderCourseModelExternal>>> Handle(GetAllProviderCoursesExternalQuery request, CancellationToken cancellationToken)
        {
            var providerCourses = await _providerCoursesReadRepository.GetAllProviderCourses(request.Ukprn);

            if (providerCourses.Count == 0)
            {
                _logger.LogInformation("ProviderCourses data not found for {Ukprn}", request.Ukprn);
                return new ValidatedResponse<List<ProviderCourseModelExternal>>(new List<ProviderCourseModelExternal>());
            }

            var standardsLookup = await _standardsReadRepository.GetAllStandards();
            var filteredProviderCourses = FilterExpiredStandards(providerCourses, standardsLookup);

            var providerCoursesModel = filteredProviderCourses.Select(p => (ProviderCourseModelExternal)p).ToList();

            if (request.ExcludeInvalidCourses)
            {
                providerCoursesModel = FilterStandardsWithoutLocations(providerCoursesModel);

                providerCoursesModel = RemoveUnapprovedRegulatedStandards(providerCoursesModel);
            }

            foreach (var p in providerCoursesModel)
            {
                var course = standardsLookup.Single(c => c.LarsCode == p.LarsCode.ToString());
                p.AttachCourseDetails(course.IfateReferenceNumber, course.Level, course.Title, course.Version,
                        course.ApprovalBody);
            }

            return new ValidatedResponse<List<ProviderCourseModelExternal>>(providerCoursesModel);
        }

        private static List<Domain.Entities.ProviderCourse> FilterExpiredStandards(List<Domain.Entities.ProviderCourse> providerCourses, List<Standard> standardsLookup)
        {
            return providerCourses
                .Where(p => standardsLookup.Select(x => x.LarsCode).Contains(p.LarsCode))
                .ToList();
        }

        private static List<ProviderCourseModelExternal> FilterStandardsWithoutLocations(
            List<ProviderCourseModelExternal> providerCourses)
        {
            return providerCourses.Where(p => p.HasLocations).ToList();
        }

        private static List<ProviderCourseModelExternal> RemoveUnapprovedRegulatedStandards(
            List<ProviderCourseModelExternal> providerCourses)
        {
            return providerCourses
                .Where(c => !c.IsRegulatedForProvider || c.IsApprovedByRegulator.GetValueOrDefault())
                .ToList();
        }
    }
}
