using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Roatp.Domain.Models;

namespace SFA.DAS.Roatp.Application.Common;

public class CourseTypeValidator : AbstractValidator<ICourseType>
{
    public const string ProviderCourseTypeNotFoundErrorMessage = "This course type or the course is not associated with the provider";
    public const string CourseNotAllowed = "This course is not in the allowed list for the provider.";

    public CourseTypeValidator(
        IProviderCourseTypesReadRepository providerCourseTypesReadRepository,
        IStandardsReadRepository standardsReadRepository,
        IProviderAllowedCoursesRepository providerAllowedCoursesRepository)
    {
        RuleFor(x => x.LarsCode)
            .Cascade(CascadeMode.Stop)
            .MustAsync(async (course, larsCode, _) =>
            {
                var standard = await standardsReadRepository.GetStandard(larsCode);
                var providerCourseTypes = await providerCourseTypesReadRepository.GetProviderCourseTypesByUkprn(course.Ukprn);

                return
                    standard != null && providerCourseTypes != null &&
                    providerCourseTypes.Any(a => a.CourseType == standard.CourseType);
            })
            .WithMessage(ProviderCourseTypeNotFoundErrorMessage)
            .MustAsync(async (course, larsCode, _) =>
            {
                var standard = await standardsReadRepository.GetStandard(larsCode);
                if (standard == null || standard.CourseType == CourseType.Apprenticeship) return true;

                List<ProviderAllowedCourse> allowedCourses = await providerAllowedCoursesRepository.GetProviderAllowedCourses(course.Ukprn, standard.CourseType, default);
                return allowedCourses.Any(a => a.LarsCode == standard.LarsCode);
            })
            .WithMessage(CourseNotAllowed);
    }
}
