using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Roatp.Domain.Models;

namespace SFA.DAS.Roatp.Application.ProviderAllowedCourses.Queries.GetProviderAllowedCourses;

public class GetProviderAllowedCoursesQueryHandler(IProviderAllowedCoursesRepository _providerAllowedCoursesRepository, IProviderCourseTypesRepository _providerCourseTypesReadRepository, IStandardsReadRepository _standardsReadRepository, IProviderCoursesReadRepository _providerCoursesReadRepository) : IRequestHandler<GetProviderAllowedCoursesQuery, GetProviderAllowedCoursesQueryResult>
{
    public async Task<GetProviderAllowedCoursesQueryResult> Handle(GetProviderAllowedCoursesQuery request, CancellationToken cancellationToken)
    {
        var providerCourseTypes = await _providerCourseTypesReadRepository.GetProviderCourseTypesByUkprn(request.Ukprn, cancellationToken);

        if (providerCourseTypes == null)
        {
            return new GetProviderAllowedCoursesQueryResult(Enumerable.Empty<ProviderAllowedCourseModel>());
        }

        var courseTypes = request.CourseType == null
            ? providerCourseTypes
            : providerCourseTypes.Where(pct => pct.CourseType == request.CourseType);

        if (!courseTypes.Any())
        {
            return new GetProviderAllowedCoursesQueryResult(Enumerable.Empty<ProviderAllowedCourseModel>());
        }

        var courses = new List<ProviderAllowedCourseModel>();

        foreach (var providerCourseType in courseTypes)
        {
            var courseTypeCourses = providerCourseType.IsRestrictedProvider
                ? await GetRestrictedCourses(request.Ukprn, providerCourseType.CourseType, cancellationToken)
                : await GetAvailableCourses(request.Ukprn, providerCourseType.CourseType);

            courses.AddRange(courseTypeCourses);
        }

        return new GetProviderAllowedCoursesQueryResult(courses);
    }

    private async Task<IEnumerable<ProviderAllowedCourseModel>> GetRestrictedCourses(int ukprn, CourseType courseType, CancellationToken cancellationToken)
    {
        var allowedCourses = await _providerAllowedCoursesRepository.GetProviderAllowedCourses(ukprn, courseType, cancellationToken);

        return allowedCourses.Select(c => (ProviderAllowedCourseModel)c);
    }

    private async Task<IEnumerable<ProviderAllowedCourseModel>> GetAvailableCourses(int ukprn, CourseType courseType)
    {
        var standards = await _standardsReadRepository.GetAllStandards();

        var providerCourses = await _providerCoursesReadRepository.GetAllProviderCourses(ukprn);

        return standards
            .Where(s => s.CourseType == courseType)
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
