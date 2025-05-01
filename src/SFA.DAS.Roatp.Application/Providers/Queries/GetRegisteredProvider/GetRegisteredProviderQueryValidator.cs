using FluentValidation;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.Providers.Queries.GetRegisteredProvider;

public class GetRegisteredProviderQueryValidator : AbstractValidator<GetRegisteredProviderQuery>
{
    public const string InvalidUkprnErrorMessage = "Invalid ukprn";
    public const string ProviderNotFoundErrorMessage = "No provider found with given ukprn";
    public GetRegisteredProviderQueryValidator(IProvidersReadRepository providersReadRepository)
    {
        RuleFor(x => x.Ukprn)
            .Cascade(CascadeMode.Stop)
            .GreaterThan(10000000).WithMessage(InvalidUkprnErrorMessage)
            .LessThan(99999999).WithMessage(InvalidUkprnErrorMessage);
    }
}
