using System.Linq;
using FluentValidation;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.Common;

public class CourseTypeUkprnValidator : AbstractValidator<ICourseTypeUkprn>
{
    public const string ProviderCourseTypeNotFoundErrorMessage = "No provider course type found with given ukprn";

    public CourseTypeUkprnValidator(IProviderCourseTypesReadRepository providerCourseTypesReadRepository)
    {
        RuleFor(c => c.Ukprn)
            .MustAsync(async (command, ukprn, cancellation) =>
            {
                var providerCourseTypes = await providerCourseTypesReadRepository.GetProviderCourseTypesByUkprn(ukprn);
                return providerCourseTypes.Any(a => a.CourseType == command.CourseType.ToString());
            })
            .WithMessage(ProviderCourseTypeNotFoundErrorMessage);
    }
}