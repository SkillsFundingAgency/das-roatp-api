using FluentValidation;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Domain.Interfaces;
using static SFA.DAS.Roatp.Domain.Constants;

namespace SFA.DAS.Roatp.Application.Courses.Queries.GetProviderDetailsForCourse
{
    public class GetProviderDetailsForCourseQueryValidator : AbstractValidator<GetProviderDetailsForCourseQuery>
    {
        public const string LatitudeOutsideUk = "The latitude entered is outside the area of the UK";
        public const string LongitudeOutsideUk = "The longitude entered is outside the area of the UK";
        public const string LatitudeAndNotLongitude = "Latitude without longitude is not valid";
        public const string NotLatitudeAndLongitude = "Longitude without latitude is not valid";

        public GetProviderDetailsForCourseQueryValidator(IProvidersReadRepository providersReadRepository, IProviderCoursesReadRepository providerCoursesReadRepository)
        {
            Include(new UkprnValidator(providersReadRepository));
            Include(new LarsCodeValidator(providersReadRepository, providerCoursesReadRepository));
            RuleFor(p => p.Lat.HasValue)
                .Equal(false)
                .When(p => p.Lat > (double)NationalLatLong.MaximumLatitude)
                .WithMessage(LatitudeOutsideUk);
            RuleFor(p => p.Lat.HasValue)
                .Equal(false)
                .When(p => p.Lat < (double)NationalLatLong.MinimumLatitude)
                .WithMessage(LatitudeOutsideUk);
            RuleFor(p => p.Lon.HasValue)
                .Equal(false)
                .When(p => p.Lon > (double)NationalLatLong.MaximumLongitude)
                .WithMessage(LongitudeOutsideUk);
            RuleFor(p => p.Lon.HasValue)
                .Equal(false)
                .When(p => p.Lon < (double)NationalLatLong.MinimumLongitude)
                .WithMessage(LongitudeOutsideUk);
            RuleFor(p => p.Lon.HasValue && !p.Lat.HasValue)
                .Equal(false)
                .WithMessage(NotLatitudeAndLongitude);
            RuleFor(p => p.Lat.HasValue && !p.Lon.HasValue)
                .Equal(false)
                .WithMessage(LatitudeAndNotLongitude);


        }
    }
}
