using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Roatp.Domain.Constants;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.ProviderAllowedCourses.Queries.GetProviderAllowedCourse;

public class GetProviderAllowedCourseDetailsQueryHandler(IProviderAllowedCoursesRepository _providerAllowedCoursesRepository) : IRequestHandler<GetProviderAllowedCourseDetailsQuery, GetProviderAllowedCourseDetailsQueryResult>
{
    public async Task<GetProviderAllowedCourseDetailsQueryResult> Handle(GetProviderAllowedCourseDetailsQuery request, CancellationToken cancellationToken)
    {
        var allowedCourse = await _providerAllowedCoursesRepository.GetProviderAllowedCourse(request.Ukprn, request.LarsCode, cancellationToken);

        if (allowedCourse == null) return null;

        return new GetProviderAllowedCourseDetailsQueryResult
        {
            LastDateStarts = allowedCourse.LastDateStarts == DateConstants.StartRestrictedDate ? null : allowedCourse.LastDateStarts,
            IsClosedToNewStarts = allowedCourse.LastDateStarts == DateConstants.StartRestrictedDate,
            IsCourseRestricted = allowedCourse.Standard.RestrictedCourseView != null,
            IsActive = allowedCourse.ProviderCourse != null
        };
    }
}
