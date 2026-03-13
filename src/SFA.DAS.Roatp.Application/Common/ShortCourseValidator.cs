using System.Linq;
using FluentValidation;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Roatp.Domain.Models;

namespace SFA.DAS.Roatp.Application.Common;

public static class ShortCourseValidator
{
    public const string MustBeAShortCourseTypeValidationMessage = "LARS code must be a valid short course";
    public const string MustBeAllowedToDeliverTheShortCourseValidationMessage = "Either the provider is not allowed to deliver ShortCourse or is not allowed to deliver the Course";
    public const string MustBeAddedToTheProviderProfileValidationMessage = "The course must be added to the provider profile";

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

    public static IRuleBuilderOptions<T, string> MustBeAddedToTheProviderProfile<T>(this IRuleBuilder<T, string> ruleBuilder, IProviderCoursesReadRepository providerCoursesReadRepository) where T : ILarsCodeUkprn
    {
        return ruleBuilder.MustAsync(async (context, larsCode, cancellation) =>
        {
            var providerCourse = await providerCoursesReadRepository.GetProviderCourseByUkprn(context.Ukprn, larsCode);

            return providerCourse != null;
        }).WithMessage(MustBeAddedToTheProviderProfileValidationMessage);
    }
}
