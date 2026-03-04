using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.ProviderAllowedCourses.Queries.GetProviderAllowedCourses;

public class GetProviderAllowedCoursesQueryHandler(IProviderAllowedCoursesRepository _providerAllowedCoursesRepository) : IRequestHandler<GetProviderAllowedCoursesQuery, GetProviderAllowedCoursesQueryResult>
{
    public async Task<GetProviderAllowedCoursesQueryResult> Handle(GetProviderAllowedCoursesQuery request, CancellationToken cancellationToken)
    {
        List<ProviderAllowedCourse> allowedCourses = await _providerAllowedCoursesRepository.GetProviderAllowedCourses(request.Ukprn, request.CourseType, cancellationToken);

        var result = allowedCourses.Select(c => (ProviderAllowedCourseModel)c);

        return new GetProviderAllowedCoursesQueryResult(result);
    }
}
