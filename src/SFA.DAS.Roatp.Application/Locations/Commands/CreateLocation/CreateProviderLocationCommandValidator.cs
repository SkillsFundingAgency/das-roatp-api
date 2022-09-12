using FluentValidation;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using System;
using System.Linq;
using static SFA.DAS.Roatp.Application.Constants;

namespace SFA.DAS.Roatp.Application.Locations.Commands.CreateLocation
{
    public class CreateProviderLocationCommandValidator : AbstractValidator<CreateProviderLocationCommand>
    {
        public const string LocationNameAlreadyUsedMessage = "The location name should be distinct.";
        public CreateProviderLocationCommandValidator(IProvidersReadRepository providerReadRepository, IProviderLocationsReadRepository providerLocationsReadRepository)
        {
            Include(new UkprnValidator(providerReadRepository));

            RuleFor(c => c.UserId).NotEmpty();
            RuleFor(c => c.LocationName)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .MaximumLength(50)
                .MustAsync(async (command, locationName, _) => 
                {
                    var locations = await providerLocationsReadRepository.GetAllProviderLocations(command.Ukprn);
                    var result = locations.Any(l => l.LocationType == LocationType.Provider && l.LocationName.Equals(locationName, StringComparison.OrdinalIgnoreCase));
                    return !result;
                })
                .WithMessage(LocationNameAlreadyUsedMessage);
            RuleFor(c => c.AddressLine1)
                .NotEmpty()
                .MaximumLength(250);
            RuleFor(c => c.AddressLine2)
                .MaximumLength(250);
            RuleFor(c => c.Town)
                .NotEmpty()
                .MaximumLength(50);
            RuleFor(c => c.Postcode)
                .NotEmpty()
                .MaximumLength(10)
                .Matches(RegularExpressions.PostcodeRegex);
            RuleFor(c => c.County)
                .MaximumLength(50);
            RuleFor(c => c.Latitude)
                .NotEmpty()
                .InclusiveBetween(-90, 90);
            RuleFor(c => c.Longitude)
                .NotEmpty()
                .InclusiveBetween(-180, 180);
            RuleFor(c => c.Email)
                .NotEmpty()
                .MaximumLength(256)
                .Matches(RegularExpressions.EmailRegex);
            RuleFor(c => c.Website)
                .NotEmpty()
                .MaximumLength(500)
                .Matches(RegularExpressions.UrlRegex);
            RuleFor(c => c.Phone)
                .NotEmpty()
                .MaximumLength(50);
        }
    }
}
