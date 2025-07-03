using FluentValidation;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.Locations.Commands.DeleteLocation;
public class DeleteProviderLocationCommandValidator : AbstractValidator<DeleteProviderLocationCommand>
{
    public const string InvalidIdErrorMessage = "Invalid id";
    public const string ProviderLocationOrphanedStandardErrorMessage = "Deleting this location will cause 1 or more standards to be without a location";

    public DeleteProviderLocationCommandValidator(IProvidersReadRepository providersReadRepository, IProviderLocationsReadRepository providerLocationsReadRepository)
    {
        Include(new UkprnValidator(providersReadRepository));

        RuleFor(p => p.Id)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage(InvalidIdErrorMessage)
            .MustAsync(async (model, id, cancellation)
                => !await providerLocationsReadRepository.DeletingWillOrphanCourses(model.Ukprn, id))
            .WithMessage(ProviderLocationOrphanedStandardErrorMessage);

        Include(new UserInfoValidator());
    }
}
