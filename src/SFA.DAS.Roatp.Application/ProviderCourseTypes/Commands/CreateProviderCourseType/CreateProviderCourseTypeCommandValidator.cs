using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.ProviderCourseTypes.Commands.CreateProviderCourseType;

public class CreateProviderCourseTypeCommandValidator : AbstractValidator<CreateProviderCourseTypeCommand>
{
    public const string CourseTypesExist = "Course types already exist for the provider";
    public const string CourseTypesNotProvided = "At least one course type must be provided";
    public const string CourseTypesDuplicated = "Course type has been duplicated";
    public CreateProviderCourseTypeCommandValidator(IProviderCourseTypesRepository providerCourseTypesRepository)
    {
        Include(new UserInfoValidator());
        RuleFor(c => c.CourseTypes)
            .NotEmpty()
            .WithMessage(CourseTypesNotProvided);
        RuleFor(c => c.CourseTypes)
            .Must(courseTypes => courseTypes.Distinct().Count() == courseTypes.Count())
            .WithMessage(CourseTypesDuplicated);
        RuleFor(x => x)
            .MustAsync(async (command, cancellation) =>
            {
                List<ProviderCourseType> providerCourseTypes = await providerCourseTypesRepository.GetProviderCourseTypesByUkprn(command.Ukprn, cancellation);

                return !providerCourseTypes.Any(x => command.CourseTypes.Contains(x.CourseType));
            })
            .WithMessage(CourseTypesExist);
    }
}
