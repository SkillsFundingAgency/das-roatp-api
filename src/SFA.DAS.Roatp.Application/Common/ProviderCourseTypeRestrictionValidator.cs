using System.Linq;
using FluentValidation;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.Common;

public class ProviderCourseTypeRestrictionValidator : AbstractValidator<IProviderCourseTypeRestriction>
{
    public const string CourseTypeNotFound = "Provider does not have course type";
    public const string CourseTypeRestricted = "Provider is restricted for course type";

    public ProviderCourseTypeRestrictionValidator(IProviderCourseTypesRepository providerCourseTypesRepository)
    {
        RuleFor(x => x)
            .CustomAsync(async (request, context, cancellation) =>
            {
                var providerCourseTypes = await providerCourseTypesRepository.GetProviderCourseTypesByUkprn(request.Ukprn, cancellation);

                var providerCourseType = providerCourseTypes.FirstOrDefault(x => x.CourseType == request.CourseType);

                if (providerCourseType == null)
                {
                    context.AddFailure(nameof(request.CourseType), $"{CourseTypeNotFound} {request.CourseType}");
                    return;
                }

                if (providerCourseType.IsRestrictedProvider)
                {
                    context.AddFailure(nameof(request.CourseType), $"{CourseTypeRestricted} {request.CourseType}");
                }
            });
    }
}

