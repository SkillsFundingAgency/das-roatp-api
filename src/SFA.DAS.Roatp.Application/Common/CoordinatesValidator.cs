using FluentValidation;

namespace SFA.DAS.Roatp.Application.Common;

public class CoordinatesValidator : AbstractValidator<ICoordinates>
{
    public const string LatitudeOutsideAcceptableRange = "The latitude entered is outside acceptable range (-90 to 90)";
    public const string LongitudeOutsideAcceptableRange = "The longitude entered is outside acceptable rangen (-180 to 180)";
    public const string LatitudeAndNotLongitude = "Latitude without longitude is not valid";
    public const string NotLatitudeAndLongitude = "Longitude without latitude is not valid";

    public CoordinatesValidator()
    {
        RuleFor(p => p.Latitude.HasValue)
            .Equal(false)
            .When(p => p.Latitude > Domain.Constants.NationalLatLong.MaximumLatitude)
            .WithMessage(LatitudeOutsideAcceptableRange);
        RuleFor(p => p.Latitude.HasValue)
            .Equal(false)
            .When(p => p.Latitude < Domain.Constants.NationalLatLong.MinimumLatitude)
            .WithMessage(LatitudeOutsideAcceptableRange);
        RuleFor(p => p.Longitude.HasValue)
            .Equal(false)
            .When(p => p.Longitude > Domain.Constants.NationalLatLong.MaximumLongitude)
            .WithMessage(LongitudeOutsideAcceptableRange);
        RuleFor(p => p.Longitude.HasValue)
            .Equal(false)
            .When(p => p.Longitude < Domain.Constants.NationalLatLong.MinimumLongitude)
            .WithMessage(LongitudeOutsideAcceptableRange);
        RuleFor(p => p.Longitude.HasValue && !p.Latitude.HasValue)
            .Equal(false)
            .WithMessage(NotLatitudeAndLongitude);
        RuleFor(p => p.Latitude.HasValue && !p.Longitude.HasValue)
            .Equal(false)
            .WithMessage(LatitudeAndNotLongitude);
    }
}