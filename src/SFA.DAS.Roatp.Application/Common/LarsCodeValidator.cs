using FluentValidation;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.Common
{
    public class LarsCodeValidator : AbstractValidator<ILarsCode>
    {
        public const string InvalidLarsCodeErrorMessage = "Invalid larsCode";
        public const string ProviderCourseNotFoundErrorMessage = "No provider course found with given ukprn and larsCode";
        public LarsCodeValidator(IProviderReadRepository providerReadRepository, IProviderCourseReadRepository providerCourseReadRepository)
        {
            RuleFor(x => x.LarsCode)
               .Cascade(CascadeMode.Stop)
               .GreaterThan(0).WithMessage(InvalidLarsCodeErrorMessage)
               .MustAsync(async (model, larsCode, cancellation) =>
               {
                   var provider = await providerReadRepository.GetByUkprn(model.Ukprn);
                   if (provider == null) return false;
                   var providerCourse = await providerCourseReadRepository.GetProviderCourse(provider.Id, larsCode);
                   return providerCourse != null;
               })
               .WithMessage(ProviderCourseNotFoundErrorMessage);
        }
    }
}
