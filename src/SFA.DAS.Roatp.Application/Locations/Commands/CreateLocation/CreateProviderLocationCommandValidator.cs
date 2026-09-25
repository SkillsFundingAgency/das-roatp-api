using System;
using System.Linq;
using FluentValidation;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Roatp.Domain.Models;
using static SFA.DAS.Roatp.Application.Constants;

namespace SFA.DAS.Roatp.Application.Locations.Commands.CreateLocation;

public class CreateProviderLocationCommandValidator : AbstractValidator<CreateProviderLocationCommand>
{
    public const string LocationNameAlreadyUsedMessage = "The location name should be distinct.";
    public CreateProviderLocationCommandValidator(IProvidersReadRepository providersReadRepository, IProviderLocationsReadRepository providerLocationsReadRepository)
    {
        Include(new UkprnValidator(providersReadRepository));

        Include(new UserInfoValidator());

        RuleFor(c => c.LocationName)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MaximumLength(50)
            .Matches(RegularExpressions.ValidCharactersRegex)
            .WithMessage(ValidationMessages.InvalidCharactersErrorMessage)
            .MustAsync(async (command, locationName, _) =>
            {
                var locations = await providerLocationsReadRepository.GetAllProviderLocations(command.Ukprn);
                var result = locations.Any(l => l.LocationType == LocationType.Provider && l.LocationName.Equals(locationName, StringComparison.OrdinalIgnoreCase));
                return !result;
            })
            .WithMessage(LocationNameAlreadyUsedMessage);
        RuleFor(c => c.AddressLine1)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MaximumLength(250)
            .Matches(RegularExpressions.ValidCharactersRegex)
            .WithMessage(ValidationMessages.InvalidCharactersErrorMessage);
        RuleFor(c => c.AddressLine2)
            .MaximumLength(250)
            .Matches(RegularExpressions.ValidCharactersRegex)
            .When(c => !string.IsNullOrWhiteSpace(c.AddressLine2), ApplyConditionTo.CurrentValidator)
            .WithMessage(ValidationMessages.InvalidCharactersErrorMessage);
        RuleFor(c => c.Town)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MaximumLength(50)
            .Matches(RegularExpressions.ValidCharactersRegex)
            .WithMessage(ValidationMessages.InvalidCharactersErrorMessage);
        RuleFor(c => c.Postcode)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MaximumLength(10)
            .Matches(RegularExpressions.ValidCharactersRegex)
            .WithMessage(ValidationMessages.InvalidCharactersErrorMessage)
            .Matches(RegularExpressions.PostcodeRegex);
        RuleFor(c => c.County)
            .Cascade(CascadeMode.Stop)
            .MaximumLength(50)
            .Matches(RegularExpressions.ValidCharactersRegex)
            .When(c => !string.IsNullOrWhiteSpace(c.County), ApplyConditionTo.CurrentValidator)
            .WithMessage(ValidationMessages.InvalidCharactersErrorMessage);
        RuleFor(c => c.Latitude)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .InclusiveBetween(-90, 90);
        RuleFor(c => c.Longitude)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .InclusiveBetween(-180, 180);
    }
}
