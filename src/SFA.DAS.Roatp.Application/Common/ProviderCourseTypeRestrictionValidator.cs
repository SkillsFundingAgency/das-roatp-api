using System.Linq;
using FluentValidation;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.Common;

public class ProviderCourseTypeRestrictionValidator : AbstractValidator<IProviderCourseTypeRestriction>
{
    public const string CourseTypeNotFound = "Requested course type does not exist for provider";
    public const string CourseTypeRestricted = "Provider is restricted for requested course type";

    public ProviderCourseTypeRestrictionValidator(IProviderCourseTypesRepository providerCourseTypesRepository)
    {
        RuleFor(x => x)
            .CustomAsync(async (request, context, cancellation) =>
            {
                var providerCourseTypes = await providerCourseTypesRepository.GetProviderCourseTypesByUkprn(request.Ukprn, cancellation);

                var providerCourseType = providerCourseTypes.FirstOrDefault(x => x.CourseType == request.CourseType);

                if (providerCourseType == null)
                {
                    context.AddFailure(CourseTypeNotFound);
                    return;
                }

                if (providerCourseType.IsRestrictedProvider)
                {
                    context.AddFailure(CourseTypeRestricted);
                }
            });
    }
}

