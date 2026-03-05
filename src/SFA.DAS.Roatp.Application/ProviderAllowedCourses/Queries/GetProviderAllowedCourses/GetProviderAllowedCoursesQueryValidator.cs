using FluentValidation;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.ProviderAllowedCourses.Queries.GetProviderAllowedCourses;

public class GetProviderAllowedCoursesQueryValidator : AbstractValidator<GetProviderAllowedCoursesQuery>
{
    public GetProviderAllowedCoursesQueryValidator(IProvidersReadRepository providersReadRepository)
    {
        Include(new UkprnValidator(providersReadRepository));
    }
}
