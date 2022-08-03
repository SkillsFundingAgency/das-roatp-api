using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FluentValidation;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.ProviderCourse
{
    public class PatchProviderCourseCommandValidator : AbstractValidator<PatchProviderCourseCommand>
    {
        public const string Replace = "replace";
        public const string NoPatchOperationsPresentErrorMessage = "There are no patch operations in this call";
        public const string PatchOperationContainsUnavailableFieldErrorMessage = "This patch operation contains an unexpected field and will not continue";
        public const string PatchOperationContainsUnavailableOperationErrorMessage = "This patch operation contains an unexpected operation and will not continue";

        public const string IsApprovedByRegulatorIsNotABooleanErrorMessage = "The patch contains an update for IsApprovedByRegulator that is not a boolean value";
        
        public const string EmailAddressTooLong = "Email address is too long, must be 256 characters or fewer";
        public const string EmailAddressWrongFormat = "Email address must be in the correct format, like name@example.com";
        public const string PhoneNumberWrongLength = "Telephone number must be between 10 and 50 characters";
        public const string ContactUsPageUrlTooLong = "Contact page address is too long, must be 500 characters or fewer";
        public const string ContactUsPageUrlWrongFormat = "Contact page address must be in the correct format, like www.example.com";
        public const string StandardInfoUrlTooLong = "Website address is too long, must be 500 characters or fewer";
        public const string StandardInfoUrlWrongFormat = "Werbsite address must be in the correct format, like www.example.com";

        public static readonly IList<string> PatchFields = new ReadOnlyCollection<string>(
                new List<string>
                {
                    "ContactUsEmail",
                    "ContactUsPhoneNumber",
                    "ContactUsPageUrl",
                     "StandardInfoUrl",
                    "IsApprovedByRegulator"
                }) ;

        public PatchProviderCourseCommandValidator(IProviderReadRepository providerReadRepository,
            IProviderCourseReadRepository providerCourseReadRepository)
        {
            Include(new UkprnValidator(providerReadRepository));

            Include(new LarsCodeValidator(providerReadRepository, providerCourseReadRepository));

            RuleFor(c => c.Patch.Operations.Count).GreaterThan(0).WithMessage(NoPatchOperationsPresentErrorMessage);

            RuleFor(c => c.Patch.Operations.Count(operation=>!PatchFields.Contains(operation.path)))
                .Equal(0)
                .WithMessage(PatchOperationContainsUnavailableFieldErrorMessage);

            RuleFor(c => c.Patch.Operations.Count(operation => !operation.op.Equals(Replace, StringComparison.CurrentCultureIgnoreCase)))
                .Equal(0)
                .WithMessage(PatchOperationContainsUnavailableOperationErrorMessage);

            RuleFor(c => c.IsApprovedByRegulator != null)
                .Equal(true)
                .When(c => c.IsPresentIsApprovedByRegulator)
                .WithMessage(IsApprovedByRegulatorIsNotABooleanErrorMessage);

            RuleFor(c => c.ContactUsEmail)
                .MaximumLength(256)
                .WithMessage(EmailAddressTooLong)
                .Matches(Constants.RegularExpressions.EmailRegex)
                .When(c=>c.IsPresentContactUsEmail)
                .WithMessage(EmailAddressWrongFormat);

            RuleFor(c => c.ContactUsPhoneNumber)
                 .MinimumLength(10)
                .WithMessage(PhoneNumberWrongLength)
                 .MaximumLength(50)
                 .WithMessage(PhoneNumberWrongLength)
                 .When(c => c.IsPresentContactUsPhoneNumber);

            RuleFor(c => c.ContactUsPageUrl)
                .MaximumLength(500)
                .WithMessage(ContactUsPageUrlTooLong)
                .Matches(Constants.RegularExpressions.UrlRegex)
                .When(c => c.IsPresentContactUsPageUrl)
                .WithMessage(ContactUsPageUrlWrongFormat);

            RuleFor(c => c.StandardInfoUrl)
                .MaximumLength(500)
                .WithMessage(StandardInfoUrlTooLong)
                .Matches(Constants.RegularExpressions.UrlRegex)
                .When(c => c.IsPresentStandardInfoUrl)
                .WithMessage(StandardInfoUrlWrongFormat);
        }
    }
}
