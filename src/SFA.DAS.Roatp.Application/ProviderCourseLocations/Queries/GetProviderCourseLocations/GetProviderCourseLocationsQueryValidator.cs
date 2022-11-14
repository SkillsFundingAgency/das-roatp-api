using FluentValidation;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.ProviderCourseLocations.Queries.GetProviderCourseLocations
{
    public class GetProviderCourseLocationsQueryValidator : AbstractValidator<GetProviderCourseLocationsQuery>
    {
        public GetProviderCourseLocationsQueryValidator(IProvidersReadRepository providersReadRepository, IProviderCoursesReadRepository providerCoursesReadRepository)
        {
            Include(new UkprnValidator(providersReadRepository));

            Include(new LarsCodeUkprnCheckerValidator(providersReadRepository, providerCoursesReadRepository));
        }
    }
}
