using System.Linq;
using FluentValidation;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.ProviderContact.Commands.CreateProviderContact;

public class CreateProviderContactCommandValidator : AbstractValidator<CreateProviderContactCommand>
{
    public const string EmailAddressTooLong = "Email address is too long, must be 300 characters or fewer";
    public const string EmailAddressWrongFormat = "Email address must be in the correct format, like name@example.com";
    public const string PhoneNumberTooLong = "Phone number is too long, must be 50 characters or fewer";
    public const string ProviderCourseIdDoesNotExist = "One or more providerCourseIds do not exist";
    public const string EmailOrPhoneNumberNeedsValue = "Email address or Phone number need to have a value";

    public CreateProviderContactCommandValidator(IProvidersReadRepository providersReadRepository, IProviderCoursesReadRepository providerCoursesReadRepository)
    {
        Include(new UserInfoValidator());
        Include(new UkprnValidator(providersReadRepository));

        RuleFor(c => c.EmailAddress)
            .MaximumLength(300)
            .WithMessage(EmailAddressTooLong)
            .Matches(Constants.RegularExpressions.EmailRegex)
            .WithMessage(EmailAddressWrongFormat)
            .When(c => !string.IsNullOrWhiteSpace(c.EmailAddress) && c.EmailAddress != string.Empty);

        RuleFor(c => c.PhoneNumber)
            .MaximumLength(50)
            .WithMessage(PhoneNumberTooLong);

        RuleFor(c => c.EmailAddress)
            .NotEmpty()
            .When(c => string.IsNullOrWhiteSpace(c.PhoneNumber) || c.PhoneNumber == string.Empty)
            .WithMessage(EmailOrPhoneNumberNeedsValue);

        RuleFor((c) => c.ProviderCourseIds)
            .MustAsync(
                async (command, providerCourseIds, cancellation) =>
                {
                    var courses = await providerCoursesReadRepository.GetAllProviderCourses(command.Ukprn);
                    return !providerCourseIds.Any(providerCourse => courses.All(l => l.Id != providerCourse));
                })
            .WithMessage(ProviderCourseIdDoesNotExist);
    }
}
