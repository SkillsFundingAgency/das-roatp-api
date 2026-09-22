using FluentValidation;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.ProviderRestrictedCourses.Queries.GetProviderNotRestrictedApprenticeships;

public class GetProviderNotRestrictedApprenticeshipsQueryValidator : AbstractValidator<GetProviderNotRestrictedApprenticeshipsQuery>
{
    public GetProviderNotRestrictedApprenticeshipsQueryValidator(IProviderCourseTypesRepository providerCourseTypesRepository)
    {
        Include(new ProviderCourseTypeRestrictionValidator(providerCourseTypesRepository));
    }
}
