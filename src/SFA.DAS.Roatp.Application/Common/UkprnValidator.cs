using FluentValidation;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.Common
{
    public class UkprnValidator : AbstractValidator<IUkprn>
    {
        public const string InvalidUkprnErrorMessage = "Invalid ukprn";
        public const string ProviderNotFoundErrorMessage = "No provider found with given ukprn";

        public UkprnValidator(IProviderReadRepository providerReadRepository)
        {
            RuleFor(x => x.Ukprn)
                .Cascade(CascadeMode.Stop)
                .GreaterThan(10000000).WithMessage(InvalidUkprnErrorMessage)
                .LessThan(99999999).WithMessage(InvalidUkprnErrorMessage)
                .MustAsync(async (ukprn, cancellation) =>
                {
                    var provider = await providerReadRepository.GetByUkprn(ukprn);
                    return provider != null;
                })
                .WithMessage(ProviderNotFoundErrorMessage);
        }
    }
}
