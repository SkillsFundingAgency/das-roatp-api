using FluentValidation;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.ProviderRestrictedCourses.Queries.GetProviderRestrictedApprenticeships;

public class GetProviderRestrictedApprenticeshipsQueryValidator : AbstractValidator<GetProviderRestrictedApprenticeshipsQuery>
{
    public GetProviderRestrictedApprenticeshipsQueryValidator(IProviderCourseTypesRepository providerCourseTypesRepository)
    {
        Include(new ProviderCourseTypeRestrictionValidator(providerCourseTypesRepository));
    }
}
