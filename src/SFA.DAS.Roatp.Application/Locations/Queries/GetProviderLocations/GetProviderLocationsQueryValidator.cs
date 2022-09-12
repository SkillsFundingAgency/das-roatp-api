using FluentValidation;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.Locations.Queries.GetProviderLocations
{
    public class GetProviderLocationsQueryValidator : AbstractValidator<GetProviderLocationsQuery>
    {
        public const string InvalidUkprnErrorMessage = "Invalid ukprn";
        public const string ProviderNotFoundErrorMessage = "No provider found with given ukprn";
        public GetProviderLocationsQueryValidator(IProvidersReadRepository providersReadRepository)
        {
            RuleFor(x => x.Ukprn)
                .Cascade(CascadeMode.Stop)
                .GreaterThan(10000000).WithMessage(InvalidUkprnErrorMessage)
                .LessThan(99999999).WithMessage(InvalidUkprnErrorMessage)
                .MustAsync(async (ukprn, cancellation) =>
                {
                    var provider = await providersReadRepository.GetByUkprn(ukprn);
                    return provider != null;
                })
                .WithMessage(ProviderNotFoundErrorMessage);
        }
    }
}
