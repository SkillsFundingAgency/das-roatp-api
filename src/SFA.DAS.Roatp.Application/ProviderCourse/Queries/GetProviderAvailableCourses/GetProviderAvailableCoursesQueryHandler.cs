using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Roatp.Application.Mediatr.Responses;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.ProviderCourse.Queries.GetProviderAvailableCourses;

public class GetProviderAvailableCoursesQueryHandler(
    IProviderCourseTypesRepository _providerCourseTypesRepository,
    IStandardsReadRepository _standardsReadRepository,
    IProviderAllowedCoursesRepository _providerAllowedCoursesRepository,
    IProviderCoursesReadRepository _providerCoursesReadRepository)
    : IRequestHandler<GetProviderAvailableCoursesQuery, ValidatedResponse<GetProviderAvailableCoursesQueryResult>>
{
    public async Task<ValidatedResponse<GetProviderAvailableCoursesQueryResult>> Handle(GetProviderAvailableCoursesQuery request, CancellationToken cancellationToken)
    {
        GetProviderAvailableCoursesQueryResult result = new();

        List<ProviderCourseType> courseTypes = await _providerCourseTypesRepository.GetProviderCourseTypesByUkprn(request.Ukprn, cancellationToken);
        var courseType = courseTypes.Find(x => x.CourseType == request.CourseType);

        if (courseType == null) return new ValidatedResponse<GetProviderAvailableCoursesQueryResult>(result);

        List<Standard> courses = await _standardsReadRepository.GetCoursesByCourseType(request.CourseType, cancellationToken);
        List<ProviderAllowedCourse> allowedCourses = await _providerAllowedCoursesRepository.GetProviderAllowedCourses(request.Ukprn, request.CourseType, cancellationToken);
        List<Domain.Entities.ProviderCourse> providerCourses = await _providerCoursesReadRepository.GetAllProviderCourses(request.Ukprn);

        IEnumerable<string> larsCodes = [];
        var includedList = providerCourses.Select(c => c.LarsCode);
        var allowedList = allowedCourses.Where(c => !c.LastDateStarts.HasValue || c.LastDateStarts.Value.Date >= DateTime.UtcNow.Date).Select(c => c.LarsCode);

        if (courseType.IsRestrictedProvider)
        {
            larsCodes = allowedList.Except(includedList);
        }
        else
        {
            var unrestrictedCourses = courses.Where(c => c.RestrictedCourseView == null).Select(c => c.LarsCode);
            var ceasedList = allowedCourses.Where(c => c.LastDateStarts.HasValue && c.LastDateStarts.Value.Date < DateTime.UtcNow.Date).Select(c => c.LarsCode);
            var unrestrictedLarsCodes = unrestrictedCourses.Except(ceasedList).Except(includedList);

            var restrictedCourses = courses.Where(c => c.RestrictedCourseView != null).Select(c => c.LarsCode);
            var allowedLarsCodes = restrictedCourses.Intersect(allowedList);
            var restrictedLarsCodes = allowedLarsCodes.Except(includedList);
            larsCodes = unrestrictedLarsCodes.Concat(restrictedLarsCodes);
        }

        result.AvailableCourses = courses
            .Where(c => larsCodes.Contains(c.LarsCode))
            .Select(c => new Common.Models.CourseBasicModel(c.LarsCode, c.Level, c.Title));

        return new ValidatedResponse<GetProviderAvailableCoursesQueryResult>(result);
    }
}
