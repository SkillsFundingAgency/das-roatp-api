using System.Linq;
using FluentValidation;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.ProviderCourse
{
    public class PatchProviderCourseCommandValidator : AbstractValidator<PatchProviderCourseCommand>
    {
        public const string NoPatchOperationsPresentErrorMessage = "There are no patch operations in this call";
        public const string NoPatchOperationsForIsApprovedByRegulatorErrorMessage = "There is an issue with the patch, the expected patch for IsApprovedByRegulator has more patch operations than expected";
        public const string TwoPatchOperationsPresentErrorMessage = "There are an invalid number of patch operations in this call (2)";
        public const string ThreePatchOperationsPresentErrorMessage = "There are an invalid number of patch operations in this call (3)";
        public const string MoreThanFourPatchOperationsPresentErrorMessage = "There are an invalid number of patch operations in this call (more than 4)";
        public const string ContactDetailsPatchErrorMessage = "There is an issue with the contact details patch";
        public const string EmailAddressTooLong = "Email address is too long, must be 256 characters or fewer";
        public const string EmailAddressWrongFormat = "Email address must be in the correct format, like name@example.com";
        public const string PhoneNumberWrongLength = "Telephone number must be between 10 and 50 characters";
        public const string ContactUsPageUrlTooLong = "Contact page address is too long, must be 500 characters or fewer";
        public const string ContactUsPageUrlWrongFormat = "Contact page address must be in the correct format, like www.example.com";
        public const string StandardInfoUrlTooLong = "Website address is too long, must be 500 characters or fewer";
        public const string StandardInfoUrlWrongFormat = "Werbsite address must be in the correct format, like www.example.com";

        private const string ContactUsEmail = "ContactUsEmail";
        private const string ContactUsPhoneNumber = "ContactUsPhoneNumber";
        private const string ContactUsPageUrl = "ContactUsPageUrl";
        private const string StandardInfoUrl = "StandardInfoUrl";

        public PatchProviderCourseCommandValidator(IProviderReadRepository providerReadRepository,
            IProviderCourseReadRepository providerCourseReadRepository)
        {
            Include(new UkprnValidator(providerReadRepository));

            Include(new LarsCodeValidator(providerReadRepository, providerCourseReadRepository));

            RuleFor(c => c.Patch.Operations.Count).GreaterThan(0).WithMessage(NoPatchOperationsPresentErrorMessage);

            RuleFor(c => c.IsPresentIsApprovedByRegulator)
                .Equal(true)
                .When(c => c.Patch.Operations.Count==1)
                .WithMessage(NoPatchOperationsForIsApprovedByRegulatorErrorMessage);

            RuleFor(c=>c.Patch.Operations.Count==2)
                .Equal(false)
                .WithMessage(TwoPatchOperationsPresentErrorMessage);

            RuleFor(c => c.Patch.Operations.Count == 3)
                .Equal(false)
                .WithMessage(ThreePatchOperationsPresentErrorMessage);

            RuleFor(c => c.Patch.Operations.Count >4)
                .Equal(false)
                .WithMessage(MoreThanFourPatchOperationsPresentErrorMessage);

            RuleFor(c => c.IsPresentContactUsEmail && c.IsPresentContactUsPageUrl && c.IsPresentContactUsPhoneNumber && c.IsPresentStandardInfoUrl)
                .Equal(true)
                .When(c => c.Patch.Operations.Count == 4)
                .WithMessage(ContactDetailsPatchErrorMessage);

            RuleFor(c => c.Patch.Operations.First(x => x.path == ContactUsEmail).value.ToString())
                .MaximumLength(256)
                .WithMessage(EmailAddressTooLong)
                .Matches(Constants.RegularExpressions.EmailRegex)
                .When(c=>c.IsPresentContactUsEmail)
                .WithMessage(EmailAddressWrongFormat);

            RuleFor(c => c.Patch.Operations.First(x => x.path == ContactUsPhoneNumber).value.ToString())
                 .MinimumLength(10)
                .WithMessage(PhoneNumberWrongLength)
                 .MaximumLength(50)
                 .WithMessage(PhoneNumberWrongLength)
                 .When(c => c.IsPresentContactUsPhoneNumber);

            RuleFor(c => c.Patch.Operations.First(x => x.path == ContactUsPageUrl).value.ToString())
                .MaximumLength(500)
                .WithMessage(ContactUsPageUrlTooLong)
                .Matches(Constants.RegularExpressions.UrlRegex)
                .When(c => c.IsPresentContactUsPageUrl)
                .WithMessage(ContactUsPageUrlWrongFormat);

            RuleFor(c => c.Patch.Operations.First(x => x.path == StandardInfoUrl).value.ToString())
                .MaximumLength(500)
                .WithMessage(StandardInfoUrlTooLong)
                .Matches(Constants.RegularExpressions.UrlRegex)
                .When(c => c.IsPresentStandardInfoUrl)
                .WithMessage(StandardInfoUrlWrongFormat);
        }
    }
}
