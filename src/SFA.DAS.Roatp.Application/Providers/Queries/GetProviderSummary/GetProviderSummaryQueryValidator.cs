using FluentValidation;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.Providers.Queries.GetProviderSummary
{
    public class GetProviderSummaryQueryValidator : AbstractValidator<GetProviderSummaryQuery>
    {
        public const string InvalidUkprnErrorMessage = "Invalid ukprn";
        public const string ProviderNotFoundErrorMessage = "No provider found with given ukprn";
        public GetProviderSummaryQueryValidator(IProvidersReadRepository providersReadRepository)
        {

            RuleFor(x => x.Ukprn)
                .Cascade(CascadeMode.Stop)
                .GreaterThan(10000000).WithMessage(InvalidUkprnErrorMessage)
                .LessThan(99999999).WithMessage(InvalidUkprnErrorMessage);
        }
    }
}
