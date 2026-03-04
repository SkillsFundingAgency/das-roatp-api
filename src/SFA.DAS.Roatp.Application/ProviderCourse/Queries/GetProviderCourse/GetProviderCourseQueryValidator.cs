using FluentValidation;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.ProviderCourse.Queries.GetProviderCourse;

public class GetProviderCourseQueryValidator : AbstractValidator<GetProviderCourseQuery>
{
    public GetProviderCourseQueryValidator(
        IProvidersReadRepository providersReadRepository,
        IProviderCoursesReadRepository providerCoursesReadRepository,
        IStandardsReadRepository standardsReadRepository,
        IProviderCourseTypesReadRepository providerCourseTypesReadRepository,
        IProviderAllowedCoursesRepository providerAllowedCoursesRepository)
    {
        Include(new UkprnValidator(providersReadRepository));
        Include(new LarsCodeUkprnCombinationValidator(providersReadRepository, providerCoursesReadRepository));
        Include(new CourseTypeValidator(providerCourseTypesReadRepository, standardsReadRepository, providerAllowedCoursesRepository));
    }
}
