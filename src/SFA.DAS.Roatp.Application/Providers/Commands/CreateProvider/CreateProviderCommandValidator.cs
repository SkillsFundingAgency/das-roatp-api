using System.Threading.Tasks;
using FluentValidation;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.Providers.Commands.CreateProvider
{
    public class CreateProviderCommandValidator : AbstractValidator<CreateProviderCommand>
    {
        public const string UkprnAlreadyPresent = "Ukprn already present";
        public const string LegalNameRequired = "Legal name is required";
        
        public CreateProviderCommandValidator(
            IProvidersReadRepository providersReadRepository)
        {
            Include(new UserInfoValidator());

            RuleFor(x => x.Ukprn)
                .Cascade(CascadeMode.Stop)
                .GreaterThan(10000000).WithMessage(UkprnValidator.InvalidUkprnErrorMessage)
                .LessThan(99999999).WithMessage(UkprnValidator.InvalidUkprnErrorMessage)
                .MustAsync(async (ukprn, cancellation) =>
                {
                    var provider = await providersReadRepository.GetByUkprn(ukprn);
                    return provider == null;
                })
                .WithMessage(UkprnAlreadyPresent);

                RuleFor((c) => c.LegalName)
                    .NotEmpty()
                    .WithMessage(LegalNameRequired);
        }
    }
}
