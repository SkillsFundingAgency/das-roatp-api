using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Roatp.Application.Mediatr.Responses;
using SFA.DAS.Roatp.Domain.Constants;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Roatp.Domain.Models;

namespace SFA.DAS.Roatp.Application.ProviderRestrictedCourses.Queries.GetProviderRestrictedApprenticeships;

public class GetProviderRestrictedApprenticeshipsQueryHandler(IStandardsReadRepository _standardsReadRepository, IProviderAllowedCoursesRepository _providerAllowedCoursesRepository) : IRequestHandler<GetProviderRestrictedApprenticeshipsQuery, ValidatedResponse<GetProviderRestrictedApprenticeshipsQueryResult>>
{
    public async Task<ValidatedResponse<GetProviderRestrictedApprenticeshipsQueryResult>> Handle(GetProviderRestrictedApprenticeshipsQuery request, CancellationToken cancellationToken)
    {
        var standards = await _standardsReadRepository.GetCoursesByCourseType(CourseType.Apprenticeship, cancellationToken);

        var providerAllowedCourses = await _providerAllowedCoursesRepository.GetProviderAllowedCourses(request.Ukprn, CourseType.Apprenticeship, cancellationToken);

        var restrictedApprenticeships = standards
            .Where(s =>
                (s.RestrictedCourseView != null &&
                 !providerAllowedCourses.Any(pac =>
                     pac.Ukprn == request.Ukprn &&
                     pac.LarsCode == s.LarsCode &&
                     pac.LastDateStarts == null))
                ||
                (s.RestrictedCourseView == null &&
                 providerAllowedCourses.Any(pac =>
                     pac.Ukprn == request.Ukprn &&
                     pac.LarsCode == s.LarsCode &&
                     pac.LastDateStarts != null)))
            .Select(standard =>
            {
                var providerAllowedCourse = providerAllowedCourses.FirstOrDefault(pac =>
                    pac.Ukprn == request.Ukprn &&
                    pac.LarsCode == standard.LarsCode);

                RestrictedApprenticeshipModel model = standard;

                if (providerAllowedCourse == null || providerAllowedCourse.LastDateStarts == DateConstants.StartRestrictedDate)
                {
                    model.LastDateStarts = null;
                    model.IsClosedToNewStarts = true;
                }
                else
                {
                    model.LastDateStarts = providerAllowedCourse.LastDateStarts;
                    model.IsClosedToNewStarts = providerAllowedCourse.LastDateStarts < DateTime.UtcNow.Date;
                }

                return model;
            })
            .ToList();

        var response = new GetProviderRestrictedApprenticeshipsQueryResult() { Courses = restrictedApprenticeships };

        return new ValidatedResponse<GetProviderRestrictedApprenticeshipsQueryResult>(response);
    }
}
