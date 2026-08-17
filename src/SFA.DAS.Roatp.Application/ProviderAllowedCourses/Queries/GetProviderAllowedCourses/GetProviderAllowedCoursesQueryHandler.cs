using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.ProviderAllowedCourses.Queries.GetProviderAllowedCourses;

public class GetProviderAllowedCoursesQueryHandler(IProviderAllowedCoursesRepository _providerAllowedCoursesRepository, IProviderCourseTypesRepository _providerCourseTypesReadRepository, IStandardsReadRepository _standardsReadRepository, IProviderCoursesReadRepository _providerCoursesReadRepository) : IRequestHandler<GetProviderAllowedCoursesQuery, GetProviderAllowedCoursesQueryResult>
{
    public async Task<GetProviderAllowedCoursesQueryResult> Handle(GetProviderAllowedCoursesQuery request, CancellationToken cancellationToken)
    {
        var providerCourseType = await _providerCourseTypesReadRepository.GetProviderCourseTypesByUkprn(request.Ukprn, cancellationToken);

        if (!providerCourseType.Any(pct => pct.CourseType == request.CourseType))
        {
            return new GetProviderAllowedCoursesQueryResult(Enumerable.Empty<ProviderAllowedCourseModel>());
        }

        bool IsProviderCourseTypeRestricted = providerCourseType.Any(pct => pct.CourseType == request.CourseType && pct.IsRestrictedProvider);

        var courses = IsProviderCourseTypeRestricted
            ? await GetRestrictedCourses(request, cancellationToken)
            : await GetAvailableCourses(request);

        return new GetProviderAllowedCoursesQueryResult(courses);
    }

    private async Task<IEnumerable<ProviderAllowedCourseModel>> GetRestrictedCourses(GetProviderAllowedCoursesQuery request, CancellationToken cancellationToken)
    {
        var allowedCourses = await _providerAllowedCoursesRepository.GetProviderAllowedCourses(request.Ukprn, request.CourseType, cancellationToken);

        return allowedCourses.Select(c => (ProviderAllowedCourseModel)c);
    }

    private async Task<IEnumerable<ProviderAllowedCourseModel>> GetAvailableCourses(GetProviderAllowedCoursesQuery request)
    {
        var standards = await _standardsReadRepository.GetAllStandards();

        var providerCourses = await _providerCoursesReadRepository.GetAllProviderCourses(request.Ukprn);

        return standards
            .Where(s => s.CourseType == request.CourseType)
            .Where(s =>
                s.RestrictedCourseView == null ||
                providerCourses.Any(pc =>
                    pc.Standard.LarsCode == s.LarsCode &&
                    pc.ProviderAllowedCourse != null))
            .Select(s => (
                Standard: s,
                ProviderCourse: providerCourses.FirstOrDefault(
                    pc => pc.Standard.LarsCode == s.LarsCode)))
            .Select(x => (ProviderAllowedCourseModel)x);
    }
}
