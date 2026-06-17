using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.RestrictedCourses.Queries.GetAllRestrictedCourses;

public class GetAllRestrictedCoursesQueryHandler(IRestrictedCourseViewRepository _restrictedCourseViewRepository, IStandardsReadRepository _standardsReadRepository) : IRequestHandler<GetAllRestrictedCoursesQuery, GetAllRestrictedCoursesQueryResult>
{
    public async Task<GetAllRestrictedCoursesQueryResult> Handle(GetAllRestrictedCoursesQuery request, CancellationToken cancellationToken)
    {
        List<RestrictedCourseView> restrictedCourses = await _restrictedCourseViewRepository.GetRestrictedCourses(cancellationToken);

        List<Standard> allStandards = await _standardsReadRepository.GetAllStandards();

        var courses = request.Restricted
        ? allStandards
            .Where(s => restrictedCourses.Any(r => r.LarsCode == s.LarsCode))
            .Select(s => (RestrictedCourseModel)s)
        : allStandards
            .Where(s => !restrictedCourses.Any(r => r.LarsCode == s.LarsCode))
            .Select(s => (RestrictedCourseModel)s);

        return new GetAllRestrictedCoursesQueryResult
        {
            Courses = courses.ToList()
        };
    }
}
