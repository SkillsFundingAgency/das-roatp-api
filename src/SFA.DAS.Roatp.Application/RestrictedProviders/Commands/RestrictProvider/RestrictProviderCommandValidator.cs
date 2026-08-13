using System.Linq;
using FluentValidation;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.RestrictedProviders.Commands.RestrictProvider;

public class RestrictProviderCommandValidator : AbstractValidator<RestrictProviderCommand>
{
    public const string ProviderNotFound = "The provider is not found for UKPRN and course type combination";
    public const string ProviderAlreadyRestricted = "The provider is already restricted for this course type";
    public RestrictProviderCommandValidator(IProvidersReadRepository providersReadRepository, IProviderCourseTypesRepository providerCourseTypesRepository)
    {
        Include(new UserInfoValidator());
        Include(new UkprnValidator(providersReadRepository));
        RuleFor(x => x)
            .CustomAsync(async (request, context, cancellation) =>
            {
                var providerCourseTypes =
                    await providerCourseTypesRepository.GetProviderCourseTypesByUkprn(request.Ukprn, cancellation);

                var providerCourseType = providerCourseTypes
                    .FirstOrDefault(x => x.CourseType == request.CourseType);

                if (providerCourseType is null)
                {
                    context.AddFailure(nameof(request.CourseType), ProviderNotFound);

                    return;
                }

                if (providerCourseType.IsRestrictedProvider)
                {
                    context.AddFailure(nameof(request.CourseType), ProviderAlreadyRestricted);
                }
            });
    }
}
