using FluentValidation;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Domain.Constants;
using SFA.DAS.Roatp.Domain.Interfaces;

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
            Include(new LatLongValidator());
        }
    }
}
