using FluentValidation;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Application.Locations.Queries;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.ProviderCourse.Queries
{
    public class ProviderAllCoursesQueryValidator : AbstractValidator<ProviderAllCoursesQuery>
    {
        public ProviderAllCoursesQueryValidator(IProviderReadRepository providerReadRepository)
        {
            Include(new UkprnValidator(providerReadRepository));
        }
    }
}
