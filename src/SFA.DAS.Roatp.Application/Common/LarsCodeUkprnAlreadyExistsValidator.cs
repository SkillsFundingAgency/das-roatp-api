using FluentValidation;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.Common
{
    public class LarsCodeUkprnAlreadyExistsValidator : AbstractValidator<ILarsCodeUkprn>
    {
        public const string InvalidMessage = "Larscode must be greater than zero";
        public const string NotFoundMessage = "Larscode not found";
        public const string CombinationAlreadyExistsMessage = "Ukprn and LarsCode combination already exists";
        public const string CombinationNotFoundErrorMessage = "Invalid Ukprn and LarsCode, combination not found";
        public LarsCodeUkprnAlreadyExistsValidator(IStandardsReadRepository standardsReadRepository, IProviderCoursesReadRepository providerCoursesReadRepository, bool expectProviderCourseToExist = false)
        {
            RuleFor(x => x.LarsCode)
                .Cascade(CascadeMode.Stop)
                .GreaterThan(0)
                .WithMessage(InvalidMessage)
                .MustAsync(async (larsCode, cancellation) =>
                {
                    var standard = await standardsReadRepository.GetStandard(larsCode);
                    return standard != null;
                })
                .WithMessage(NotFoundMessage)
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
