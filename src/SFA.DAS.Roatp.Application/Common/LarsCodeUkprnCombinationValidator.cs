using FluentValidation;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.Common
{
    public class LarsCodeUkprnCombinationValidator : AbstractValidator<ILarsCodeUkprn>
    {
        public const string CombinationAlreadyExistsMessage = "Ukprn and LarsCode combination already exists";
        public const string CombinationNotFoundErrorMessage = "Invalid Ukprn and LarsCode, combination not found";
        public LarsCodeUkprnCombinationValidator( IProviderCoursesReadRepository providerCoursesReadRepository, bool expectProviderCourseToExist = false)
        {
            RuleFor(x => x.LarsCode)
                .Cascade(CascadeMode.Stop)
                .MustAsync(async (model, larsCode, cancellation) =>
                {
                    var providerCourse = await providerCoursesReadRepository.GetProviderCourseByUkprn(model.Ukprn, larsCode);
                    return providerCourse != null;
                })
                .WithMessage(CombinationNotFoundErrorMessage)
                .When(_ => expectProviderCourseToExist, ApplyConditionTo.CurrentValidator)
                .MustAsync(async (model, larsCode, cancellation) =>
                {
                    var providerCourse = await providerCoursesReadRepository.GetProviderCourseByUkprn(model.Ukprn, larsCode);
                    return providerCourse == null;
                })
                .WithMessage(CombinationAlreadyExistsMessage)
                .When(_ => !expectProviderCourseToExist, ApplyConditionTo.CurrentValidator);
        }
    }
}
