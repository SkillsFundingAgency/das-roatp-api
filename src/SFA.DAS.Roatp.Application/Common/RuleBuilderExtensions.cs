using FluentValidation;
using static SFA.DAS.Roatp.Application.Common.ValidationMessages;

namespace SFA.DAS.Roatp.Application.Common
{
    public static class RuleBuilderExtensions
    {
        public static IRuleBuilderOptions<T, string> MustBeValidEmail<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder
                .MaximumLength(256)
                .WithMessage(EmailValidationMessages.EmailAddressTooLong)
                .Matches(Constants.RegularExpressions.EmailRegex)
                .WithMessage(EmailValidationMessages.EmailAddressWrongFormat);
        }

        public static IRuleBuilderOptions<T, string> MustBeValidPhoneNumber<T>(this IRuleBuilder<T, string> ruleBuilder) =>
            ruleBuilder
                .MinimumLength(10)
                .WithMessage(PhoneNumberValidationMessages.PhoneNumberWrongLength)
                .MaximumLength(50)
                .WithMessage(PhoneNumberValidationMessages.PhoneNumberWrongLength);

        public static IRuleBuilderOptions<T, string> MustBeValidUrl<T>(this IRuleBuilder<T, string> ruleBuilder, string fieldName) =>
            ruleBuilder
                .MaximumLength(500)
                .WithMessage(UrlValidationMessages.UrlTooLong(fieldName))
                .Matches(Constants.RegularExpressions.UrlRegex)
                .WithMessage(UrlValidationMessages.UrlWrongFormat(fieldName));

    }
}
