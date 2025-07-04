using FluentValidation;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.Locations.Commands.DeleteLocation;
public class DeleteProviderLocationCommandValidator : AbstractValidator<DeleteProviderLocationCommand>
{
    public const string InvalidIdErrorMessage = "Invalid id";
    public const string ProviderLocationNotFoundErrorMessage = "No provider location found with given ukprn and id";

    // MFCMFC need a validator rule/repo for not removing locations that will leave a standard location-less
    public DeleteProviderLocationCommandValidator(IProvidersReadRepository providersReadRepository, IProviderLocationsReadRepository providerLocationsReadRepository)
    {
        Include(new UkprnValidator(providersReadRepository));

        RuleFor(p => p.Id)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage(InvalidIdErrorMessage)
            .MustAsync(async (model, id, cancellation) =>
            {
                var providerLocation = await providerLocationsReadRepository.GetProviderLocation(model.Ukprn, id);
                return providerLocation != null;
            })
            .WithMessage(ProviderLocationNotFoundErrorMessage);

        Include(new UserInfoValidator());
    }
}
