using FluentValidation;

namespace SFA.DAS.Roatp.Application.Common;

public class LatLongValidator : AbstractValidator<ILatLon>
{
    public const string LatitudeOutsideUk = "The latitude entered is outside the area of the UK";
    public const string LongitudeOutsideUk = "The longitude entered is outside the area of the UK";
    public const string LatitudeAndNotLongitude = "Latitude without longitude is not valid";
    public const string NotLatitudeAndLongitude = "Longitude without latitude is not valid";

    public LatLongValidator()
    {
        RuleFor(p => p.Latitude.HasValue)
            .Equal(false)
            .When(p => p.Latitude > (double)Domain.Constants.NationalLatLong.MaximumLatitude)
            .WithMessage(LatitudeOutsideUk);
        RuleFor(p => p.Latitude.HasValue)
            .Equal(false)
            .When(p => p.Latitude < (double)Domain.Constants.NationalLatLong.MinimumLatitude)
            .WithMessage(LatitudeOutsideUk);
        RuleFor(p => p.Longitude.HasValue)
            .Equal(false)
            .When(p => p.Longitude > (double)Domain.Constants.NationalLatLong.MaximumLongitude)
            .WithMessage(LongitudeOutsideUk);
        RuleFor(p => p.Longitude.HasValue)
            .Equal(false)
            .When(p => p.Longitude < (double)Domain.Constants.NationalLatLong.MinimumLongitude)
            .WithMessage(LongitudeOutsideUk);
        RuleFor(p => p.Longitude.HasValue && !p.Latitude.HasValue)
            .Equal(false)
            .WithMessage(NotLatitudeAndLongitude);
        RuleFor(p => p.Latitude.HasValue && !p.Longitude.HasValue)
            .Equal(false)
            .WithMessage(LatitudeAndNotLongitude);
    }
}