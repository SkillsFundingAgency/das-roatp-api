using FluentValidation;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.ProviderCourseLocations.Queries.GetProviderCourseLocations
{
    public class GetProviderCourseLocationsQueryValidator : AbstractValidator<GetProviderCourseLocationsQuery>
    {
        public GetProviderCourseLocationsQueryValidator(IProviderReadRepository providerReadRepository, IProviderCourseReadRepository providerCourseReadRepository)
        {
            Include(new UkprnValidator(providerReadRepository));

            Include(new LarsCodeValidator(providerReadRepository, providerCourseReadRepository));
        }
    }
}
