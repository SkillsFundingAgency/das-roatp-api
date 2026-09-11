using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Roatp.Domain.Models;

namespace SFA.DAS.Roatp.Application.ProviderRestrictedCourses.Queries.GetProviderRestrictedApprenticeships;

public class GetProviderRestrictedApprenticeshipsQueryValidator : AbstractValidator<GetProviderRestrictedApprenticeshipsQuery>
{
    private const string ProviderNotRestricted = "Provider is not restricted for Apprenticeships";
    public GetProviderRestrictedApprenticeshipsQueryValidator(IProviderCourseTypesRepository _providerCourseTypesRepository)
    {
        RuleFor(x => x)
            .MustAsync(async (request, cancellation) =>
            {
                List<ProviderCourseType> providerCourseTypes = await _providerCourseTypesRepository.GetProviderCourseTypesByUkprn(request.Ukprn, cancellation);

                return providerCourseTypes.Any(x => x.CourseType == CourseType.Apprenticeship && x.IsRestrictedProvider);
            })
            .WithMessage(ProviderNotRestricted);
    }
}
