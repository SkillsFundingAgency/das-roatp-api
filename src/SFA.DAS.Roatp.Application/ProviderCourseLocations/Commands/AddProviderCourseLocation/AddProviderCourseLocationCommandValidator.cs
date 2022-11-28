using FluentValidation;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.ProviderCourseLocations.Commands.AddProviderCourseLocation
{
    public class AddProviderCourseLocationCommandValidator : AbstractValidator<AddProviderCourseLocationCommand>
    {
        public const string LocationNavigationIdErrorMessage = "Location id is invalid or not found";
        public const string LocationAlreadyExistsErrorMessage = "Location already exists on Provider course";
        public const string TrainingVenueErrorMessage = "Venue must be provided";
        public const string DeliveryMethodErrorMessage = "Delivery method must be provided";
        public AddProviderCourseLocationCommandValidator
            (IProvidersReadRepository providersReadRepository, 
             IProviderCoursesReadRepository providerCoursesReadRepository, 
             IProviderLocationsReadRepository providerLocationsReadRepository,
             IProviderCourseLocationsReadRepository providerCourseLocationsReadRepository)
        {
            Include(new UkprnValidator(providersReadRepository));

            Include(new LarsCodeValidator(providersReadRepository, providerCoursesReadRepository));

            Include(new UserInfoValidator());

            RuleFor(c => c.LocationNavigationId)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage(TrainingVenueErrorMessage)
                .MustAsync(async (command, locationNavigationId, _) =>
                {
                    var providerLocation = await providerLocationsReadRepository.GetProviderLocation(command.Ukprn, command.LocationNavigationId);
                    return providerLocation != null;
                })
                .WithMessage(LocationNavigationIdErrorMessage)
                .MustAsync(async (command, locationNavigationId, _) =>
                {
                    var providerLocation = await providerLocationsReadRepository.GetProviderLocation(command.Ukprn, command.LocationNavigationId);
                    var providerCourseLocations = await providerCourseLocationsReadRepository.GetAllProviderCourseLocations(command.Ukprn, command.LarsCode);
                    return !providerCourseLocations.Exists(l => l.ProviderLocationId == providerLocation.Id);
                })
                .WithMessage(LocationAlreadyExistsErrorMessage);

            RuleFor(x => x.HasDayReleaseDeliveryOption).Equal(true)
               .When(a => a.HasBlockReleaseDeliveryOption.HasValue && a.HasBlockReleaseDeliveryOption == false)
               .WithMessage(DeliveryMethodErrorMessage);

        }
    }
}
