using System.Linq;
using FluentValidation;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Roatp.Domain.Models;

namespace SFA.DAS.Roatp.Application.Common;

public static class ShortCourseValidator
{
    public const string MustBeAShortCourseTypeValidationMessage = "LARS code must be for a short course";
    public const string MustBeAllowedToDeliverTheShortCourseValidationMessage = "The provider should be allowed to deliver ShortCourses and the given ShortCourse";

    public static IRuleBuilderOptions<T, string> MustBeAShortCourseType<T>(this IRuleBuilder<T, string> ruleBuilder, IStandardsReadRepository standardsReadRepository) where T : ILarsCode
    {
        return ruleBuilder.MustAsync(async (larsCode, cancellation) =>
        {
            var standard = await standardsReadRepository.GetStandard(larsCode);
            return standard != null && standard.CourseType == CourseType.ShortCourse;
        }).WithMessage(MustBeAShortCourseTypeValidationMessage);
    }

    /// <summary>
    /// Use this in conjunction with MustBeAShortCourseType to check that the provider is allowed to deliver the short course. 
    /// </summary>
    public static IRuleBuilderOptions<T, string> MustBeAllowedToDeliverTheShortCourse<T>(this IRuleBuilder<T, string> ruleBuilder, IStandardsReadRepository standardsReadRepository, IProviderCourseTypesReadRepository providerCourseTypesReadRepository, IProviderAllowedCoursesRepository providerAllowedCoursesRepository) where T : ILarsCodeUkprn
    {
        return ruleBuilder.MustAsync(async (context, larsCode, cancellation) =>
        {
            var standard = await standardsReadRepository.GetStandard(larsCode);
            if (standard.CourseType != CourseType.ShortCourse) return false;

            var courseTypes = await providerCourseTypesReadRepository.GetProviderCourseTypesByUkprn(context.Ukprn, cancellation);
            var allowedCourses = await providerAllowedCoursesRepository.GetProviderAllowedCourses(context.Ukprn, CourseType.ShortCourse, cancellation);

            return courseTypes.Any(ct => ct.CourseType == standard.CourseType) && allowedCourses.Any(ac => ac.LarsCode == larsCode);
        }).WithMessage(MustBeAllowedToDeliverTheShortCourseValidationMessage);
    }
}
