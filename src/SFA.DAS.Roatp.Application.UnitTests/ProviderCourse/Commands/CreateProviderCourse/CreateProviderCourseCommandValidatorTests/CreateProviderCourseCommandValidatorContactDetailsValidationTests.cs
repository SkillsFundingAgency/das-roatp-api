using FluentValidation.TestHelper;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Application.ProviderCourse.Commands.CreateProviderCourse;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Application.UnitTests.ProviderCourse.Commands.CreateProviderCourse.CreateProviderCourseCommandValidatorTests
{
    [TestFixture]
    public class CreateProviderCourseCommandValidatorContactDetailsValidationTests : CreateProviderCourseCommandValidatorTestBase
    {
        [TestCase("ab.cd@ef.com", true)]
        [TestCase("", false)]
        public async Task Email_Required_Validation(string email, bool isValid)
        {
            var command = new CreateProviderCourseCommand { ContactUsEmail = email };
            var sut = GetSut();

            var result = await sut.TestValidateAsync(command);

            if (isValid)
                result.ShouldNotHaveValidationErrorFor(c => c.ContactUsEmail);
            else
                result.ShouldHaveValidationErrorFor(c => c.ContactUsEmail).WithErrorMessage(ValidationMessages.IsRequired(nameof(CreateProviderCourseCommand.ContactUsEmail)));
        }

        [TestCase("ab.cd@ef.com", true)]
        [TestCase("invalidemail", false)]
        public async Task Email_Format_Validation(string email, bool isValid)
        {
            var command = new CreateProviderCourseCommand { ContactUsEmail = email };
            var sut = GetSut();

            var result = await sut.TestValidateAsync(command);

            if (isValid)
                result.ShouldNotHaveValidationErrorFor(c => c.ContactUsEmail);
            else
                result.ShouldHaveValidationErrorFor(c => c.ContactUsEmail).WithErrorMessage(ValidationMessages.EmailValidationMessages.EmailAddressWrongFormat);
        }

        [TestCase("1234567890", true)]
        [TestCase("", false)]
        public async Task Phone_Required_Validation(string phone, bool isValid)
        {
            var command = new CreateProviderCourseCommand { ContactUsPhoneNumber = phone };
            var sut = GetSut();

            var result = await sut.TestValidateAsync(command);

            if (isValid)
                result.ShouldNotHaveValidationErrorFor(c => c.ContactUsPhoneNumber);
            else
                result.ShouldHaveValidationErrorFor(c => c.ContactUsPhoneNumber).WithErrorMessage(ValidationMessages.IsRequired(nameof(CreateProviderCourseCommand.ContactUsPhoneNumber)));
        }

        [TestCase("1234567890", true)]
        [TestCase("231", false)]
        public async Task Phone_Format_Validation(string phone, bool isValid)
        {
            var command = new CreateProviderCourseCommand { ContactUsPhoneNumber = phone };
            var sut = GetSut();

            var result = await sut.TestValidateAsync(command);

            if (isValid)
                result.ShouldNotHaveValidationErrorFor(c => c.ContactUsPhoneNumber);
            else
                result.ShouldHaveValidationErrorFor(c => c.ContactUsPhoneNumber).WithErrorMessage(ValidationMessages.PhoneNumberValidationMessages.PhoneNumberWrongLength);
        }

        [TestCase("www.goo.com", true)]
        [TestCase("", false)]
        public async Task PageUrl_Required_Validation(string pageUrl, bool isValid)
        {
            var command = new CreateProviderCourseCommand { ContactUsPageUrl = pageUrl };
            var sut = GetSut();

            var result = await sut.TestValidateAsync(command);

            if (isValid)
                result.ShouldNotHaveValidationErrorFor(c => c.ContactUsPageUrl);
            else
                result.ShouldHaveValidationErrorFor(c => c.ContactUsPageUrl).WithErrorMessage(ValidationMessages.IsRequired(nameof(CreateProviderCourseCommand.ContactUsPageUrl)));
        }

        [TestCase("www.goo.com", true)]
        [TestCase("invalidPageUrl", false)]
        public async Task PageUrl_Format_Validation(string pageUrl, bool isValid)
        {
            var command = new CreateProviderCourseCommand { ContactUsPageUrl = pageUrl };
            var sut = GetSut();

            var result = await sut.TestValidateAsync(command);

            if (isValid)
                result.ShouldNotHaveValidationErrorFor(c => c.ContactUsPageUrl);
            else
                result.ShouldHaveValidationErrorFor(c => c.ContactUsPageUrl).WithErrorMessage(ValidationMessages.UrlValidationMessages.UrlWrongFormat("Contact page"));
        }

        [TestCase("www.goo.com", true)]
        [TestCase("", false)]
        public async Task StandardInfoUrl_Required_Validation(string standardInfoUrl, bool isValid)
        {
            var command = new CreateProviderCourseCommand { StandardInfoUrl = standardInfoUrl };
            var sut = GetSut();

            var result = await sut.TestValidateAsync(command);

            if (isValid)
                result.ShouldNotHaveValidationErrorFor(c => c.StandardInfoUrl);
            else
                result.ShouldHaveValidationErrorFor(c => c.StandardInfoUrl).WithErrorMessage(ValidationMessages.IsRequired(nameof(CreateProviderCourseCommand.StandardInfoUrl)));
        }

        [TestCase("www.goo.com", true)]
        [TestCase("invalidStandardInfoUrl", false)]
        public async Task StandardInfoUrl_Format_Validation(string standardInfoUrl, bool isValid)
        {
            var command = new CreateProviderCourseCommand { StandardInfoUrl = standardInfoUrl };
            var sut = GetSut();

            var result = await sut.TestValidateAsync(command);

            if (isValid)
                result.ShouldNotHaveValidationErrorFor(c => c.StandardInfoUrl);
            else
                result.ShouldHaveValidationErrorFor(c => c.StandardInfoUrl).WithErrorMessage(ValidationMessages.UrlValidationMessages.UrlWrongFormat("Website"));
        }
    }
}
