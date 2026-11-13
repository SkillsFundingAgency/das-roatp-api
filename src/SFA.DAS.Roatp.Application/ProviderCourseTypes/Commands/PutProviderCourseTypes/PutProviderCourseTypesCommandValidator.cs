using System.Linq;
using FluentValidation;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.ProviderCourseTypes.Commands.PutProviderCourseTypes;

public class PutProviderCourseTypesCommandValidator : AbstractValidator<PutProviderCourseTypesCommand>
{
    public const string CourseTypesNotProvided = "At least one course type must be provided";
    public const string CourseTypesDuplicated = "Course type has been duplicated";
    public const string CourseTypeInvalid = "Course type is not a valid value";
    public const string ProviderCourseTypesNotFound = "No course types exist for the provider";

    public PutProviderCourseTypesCommandValidator(IProviderCourseTypesRepository providerCourseTypesRepository)
    {
        Include(new UserInfoValidator());
        RuleFor(x => x.Ukprn)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .GreaterThan(10000000).WithMessage(UkprnValidator.InvalidUkprnErrorMessage)
            .LessThan(19999999).WithMessage(UkprnValidator.InvalidUkprnErrorMessage);
        RuleFor(c => c.CourseTypes)
            .NotEmpty()
            .WithMessage(CourseTypesNotProvided);
        RuleForEach(c => c.CourseTypes)
            .IsInEnum()
            .WithMessage(CourseTypeInvalid);
        RuleFor(c => c.CourseTypes)
            .Must(courseTypes => courseTypes.Distinct().Count() == courseTypes.Count())
            .When(c => c.CourseTypes != null)
            .WithMessage(CourseTypesDuplicated);
        RuleFor(x => x.Ukprn)
            .MustAsync(async (ukprn, cancellation) =>
            {
                var providerCourseTypes = await providerCourseTypesRepository.GetProviderCourseTypesByUkprn(ukprn, cancellation);
                return providerCourseTypes.Count > 0;
            })
            .WithMessage(ProviderCourseTypesNotFound);
    }
}
