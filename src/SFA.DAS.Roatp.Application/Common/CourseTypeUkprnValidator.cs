using System.Linq;
using FluentValidation;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.Common;

public static class CourseTypeUkprnValidator
{
    public const string ProviderCourseTypeNotFoundErrorMessage = "No provider course type found with given ukprn";

    public static IRuleBuilderOptions<T, CourseTypeUkprnValidationObject> ValidateCourseTypeForUkprn<T>(
        this IRuleBuilderInitial<T, CourseTypeUkprnValidationObject> ruleBuilder,
        IProviderCourseTypesReadRepository providerCourseTypesReadRepository
    ) where T : IUkprn, ICourseType
    {
        return ruleBuilder
            .MustAsync(async (validationObject, cancellationToken) =>
            {
                var providerCourseTypes =
                    await providerCourseTypesReadRepository.GetProviderCourseTypesByUkprn(validationObject.Ukprn);
                return providerCourseTypes != null && providerCourseTypes.Any(a => a.CourseType == validationObject.CourseType);
            })
            .WithMessage(ProviderCourseTypeNotFoundErrorMessage);
    }
}