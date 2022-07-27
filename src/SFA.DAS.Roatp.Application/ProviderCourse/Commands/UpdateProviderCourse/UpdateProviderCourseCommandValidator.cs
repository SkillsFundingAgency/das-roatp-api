using FluentValidation;
using static SFA.DAS.Roatp.Application.Constants;

namespace SFA.DAS.Roatp.Application.ProviderCourse.Commands.UpdateProviderCourse
{
    public class UpdateProviderCourseCommandValidator : AbstractValidator<UpdateProviderCourseCommand>
    {
        public UpdateProviderCourseCommandValidator()
        {
            RuleFor(c => c.Ukprn)
                .GreaterThan(10000000)
                .LessThan(99999999);
            RuleFor(c => c.LarsCode)
                .GreaterThan(0);
            RuleFor(c => c.UserId)
                .NotEmpty();
            RuleFor(p => p.ContactUsEmail)
                .MaximumLength(256)
                .Matches(RegularExpressions.EmailRegex);
            RuleFor(p => p.ContactUsPhoneNumber)
                .MinimumLength(10)
                .MaximumLength(50);
            RuleFor(p => p.ContactUsPageUrl)
                .MaximumLength(500)
                .Matches(RegularExpressions.UrlRegex);
            RuleFor(p => p.StandardInfoUrl)
                .MaximumLength(500)
                .Matches(RegularExpressions.UrlRegex);
        }
    }
}
