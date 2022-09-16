using FluentValidation;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.Locations.Queries.GetProviderLocationDetails
{
    public class GetProviderLocationDetailsQueryValidator : AbstractValidator<GetProviderLocationDetailsQuery>
    {
        public const string InvalidIdErrorMessage = "Invalid id";
        public const string ProviderLocationNotFoundErrorMessage = "No provider location found with given ukprn and id";
        public GetProviderLocationDetailsQueryValidator(IProvidersReadRepository providersReadRepository, IProviderLocationsReadRepository providerLocationsReadRepository)
        {
            Include(new UkprnValidator(providersReadRepository));

            RuleFor(x => x.Id)
               .Cascade(CascadeMode.Stop)
               .NotEmpty().WithMessage(InvalidIdErrorMessage)
               .MustAsync(async (model, id , cancellation) =>
               {
                   var providerLocation = await providerLocationsReadRepository.GetProviderLocation(model.Ukprn, id);
                   return providerLocation != null;
               })
               .WithMessage(ProviderLocationNotFoundErrorMessage);
        }
    }
}
