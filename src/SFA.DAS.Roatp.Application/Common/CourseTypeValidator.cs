using System.Linq;
using FluentValidation;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.Common;

public class CourseTypeValidator : AbstractValidator<ICourseType>
{
    public const string ProviderCourseTypeNotFoundErrorMessage = "No provider course type found with given ukprn";

    public CourseTypeValidator(IProviderCourseTypesReadRepository providerCourseTypesReadRepository,
        IStandardsReadRepository standardsReadRepository)
    {
        RuleFor(x => x.LarsCode)
            .Cascade(CascadeMode.Stop)
            .MustAsync(async (course, larsCode, cancellationToken) =>
                {
                    var standard = await standardsReadRepository.GetStandard(larsCode);
                    var providerCourseTypes =
                                         await providerCourseTypesReadRepository.GetProviderCourseTypesByUkprn(course.Ukprn);
                    return standard != null && providerCourseTypes != null &&
                           providerCourseTypes.Any(a => a.CourseType == standard.CourseType);

                })
                .WithMessage(ProviderCourseTypeNotFoundErrorMessage);
    }
}
