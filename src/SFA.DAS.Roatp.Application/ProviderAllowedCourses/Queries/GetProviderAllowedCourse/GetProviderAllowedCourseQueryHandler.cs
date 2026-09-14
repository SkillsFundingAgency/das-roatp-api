using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Roatp.Domain.Constants;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.ProviderAllowedCourses.Queries.GetProviderAllowedCourse;

public class GetProviderAllowedCourseQueryHandler(IProviderAllowedCoursesRepository _providerAllowedCoursesRepository) : IRequestHandler<GetProviderAllowedCourseQuery, GetProviderAllowedCourseQueryResult>
{
    public async Task<GetProviderAllowedCourseQueryResult> Handle(GetProviderAllowedCourseQuery request, CancellationToken cancellationToken)
    {
        var allowedCourse = await _providerAllowedCoursesRepository.GetProviderAllowedCourse(request.Ukprn, request.LarsCode, cancellationToken);

        if (allowedCourse == null) return null;

        return new GetProviderAllowedCourseQueryResult
        {
            LastDateStarts = allowedCourse.LastDateStarts == DateConstants.StartRestrictedDate ? null : allowedCourse.LastDateStarts,
            IsClosedToNewStarts = allowedCourse.LastDateStarts == DateConstants.StartRestrictedDate,
            IsCourseRestricted = allowedCourse.Standard.RestrictedCourseView != null,
            IsActive = allowedCourse.ProviderCourse != null
        };
    }
}
