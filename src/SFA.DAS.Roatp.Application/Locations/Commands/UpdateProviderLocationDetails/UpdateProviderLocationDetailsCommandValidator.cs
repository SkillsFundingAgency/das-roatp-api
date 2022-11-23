using FluentValidation;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Roatp.Domain.Models;
using System;
using System.Linq;
using static SFA.DAS.Roatp.Application.Constants;

namespace SFA.DAS.Roatp.Application.Locations.Commands.UpdateProviderLocationDetails
{
    public class UpdateProviderLocationDetailsCommandValidator : AbstractValidator<UpdateProviderLocationDetailsCommand>
    {
        public const string InvalidIdErrorMessage = "Invalid id";
        public const string ProviderLocationNotFoundErrorMessage = "No provider location found with given ukprn and id";
        public const string LocationNameAlreadyUsedMessage = "The location name should be distinct.";
        public UpdateProviderLocationDetailsCommandValidator(IProvidersReadRepository providersReadRepository, IProviderLocationsReadRepository providerLocationsReadRepository)
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

            RuleFor(p => p.LocationName)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .MaximumLength(50)
                .MustAsync(async (model, locationName, _) =>
                {
                    var locations = await providerLocationsReadRepository.GetAllProviderLocations(model.Ukprn);
                    var result = locations.Where(a => a.NavigationId != model.Id).Any(l => l.LocationType == LocationType.Provider && l.LocationName.Equals(locationName, StringComparison.OrdinalIgnoreCase));
                    return !result;
                })
                .WithMessage(LocationNameAlreadyUsedMessage);

            RuleFor(p => p.Email)
                .NotEmpty()
                .MaximumLength(256)
                .Matches(RegularExpressions.EmailRegex);

            RuleFor(p => p.Phone)
                .NotEmpty()
                .MinimumLength(10)
                .MaximumLength(50);

            RuleFor(p => p.Website)
                .NotEmpty()
                .MaximumLength(500)
                .Matches(RegularExpressions.UrlRegex);
        }
    }
}
