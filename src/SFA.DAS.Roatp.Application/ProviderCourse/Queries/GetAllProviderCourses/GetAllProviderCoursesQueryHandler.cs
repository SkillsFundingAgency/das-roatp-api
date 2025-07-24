using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Application.Mediatr.Responses;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.ProviderCourse.Queries.GetAllProviderCourses
{
    public class GetAllProviderCoursesQueryHandler : IRequestHandler<GetAllProviderCoursesQuery, ValidatedResponse<List<ProviderCourseModel>>>
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

        public async Task<ValidatedResponse<List<ProviderCourseModel>>> Handle(GetAllProviderCoursesQuery request, CancellationToken cancellationToken)
        {
            var providerCourses = await _providerCoursesReadRepository.GetAllProviderCourses(request.Ukprn);

            if (!providerCourses.Any())
            {
                _logger.LogInformation("ProviderCourses data not found for {ukprn}", request.Ukprn);
                return new ValidatedResponse<List<ProviderCourseModel>>(new List<ProviderCourseModel>());
            }

            var standardsLookup = await _standardsReadRepository.GetAllStandards();
            var filteredProviderCourses = FilterExpiredStandards(providerCourses, standardsLookup);

            var providerCoursesModel = filteredProviderCourses.Select(p => (ProviderCourseModel)p).ToList();

            if (request.ExcludeInvalidCourses)
            {
                providerCoursesModel = FilterStandardsWithoutLocations(providerCoursesModel);

                providerCoursesModel = RemoveUnapprovedRegulatedStandards(providerCoursesModel);
            }

            foreach (var p in providerCoursesModel)
            {
                var course = standardsLookup.Single(c => c.LarsCode == p.LarsCode);
                p.AttachCourseDetails(course.IfateReferenceNumber, course.Level, course.Title, course.Version,
                        course.ApprovalBody);
            }

            return new ValidatedResponse<List<ProviderCourseModel>>(providerCoursesModel);
        }

        private static List<Domain.Entities.ProviderCourse> FilterExpiredStandards(List<Domain.Entities.ProviderCourse> providerCourses, List<Standard> standardsLookup)
        {
            return providerCourses
                .Where(p => standardsLookup.Select(x => x.LarsCode).Contains(p.LarsCode))
                .ToList();
        }

        private List<ProviderCourseModel> FilterStandardsWithoutLocations(
            List<ProviderCourseModel> providerCourses)
        {
            return providerCourses.Where(p => p.HasLocations).ToList();
        }

        private static List<ProviderCourseModel> RemoveUnapprovedRegulatedStandards(
            List<ProviderCourseModel> providerCourses)
        {
            return providerCourses
                .Where(c => !c.IsRegulatedForProvider || c.IsApprovedByRegulator.GetValueOrDefault())
                .ToList();
        }
    }
}
