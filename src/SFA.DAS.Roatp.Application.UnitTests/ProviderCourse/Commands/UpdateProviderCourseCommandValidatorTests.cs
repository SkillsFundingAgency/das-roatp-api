using FluentAssertions;
using FluentValidation.TestHelper;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.ProviderCourse;

namespace SFA.DAS.Roatp.Application.UnitTests.ProviderCourse.Commands
{
    [TestFixture]
    public class UpdateProviderCourseCommandValidatorTests
    {
        [TestCase(10000000, false)]
        [TestCase(10000001, true)]
        [TestCase(100000000, false)]
        public void Validate_Ukprn(int ukprn, bool isValid)
        {
            var validator = new UpdateProviderCourseCommandValidator();

            var result = validator.TestValidate(new UpdateProviderCourseCommand { Ukprn = ukprn });

            if (isValid)
                result.ShouldNotHaveValidationErrorFor(c => c.Ukprn);
            else
                result.ShouldHaveValidationErrorFor(c => c.Ukprn);
        }

        [TestCase(0, false)]
        [TestCase(1, true)]
        [TestCase(-1, false)]
        public void Validate_LarsCode(int larsCode, bool isValid)
        {
            var validator = new UpdateProviderCourseCommandValidator();

            var result = validator.TestValidate(new UpdateProviderCourseCommand { LarsCode = larsCode });

            if (isValid)
                result.ShouldNotHaveValidationErrorFor(c => c.LarsCode);
            else
                result.ShouldHaveValidationErrorFor(c => c.LarsCode);
        }

        [TestCase("", false)]
        [TestCase(null, false)]
        [TestCase(" ", false)]
        [TestCase("DATA", true)]
        public void Validate_UserId(string userId, bool isValid)
        {
            var validator = new UpdateProviderCourseCommandValidator();

            var result = validator.TestValidate(new UpdateProviderCourseCommand { UserId = userId });

            if (isValid)
                result.ShouldNotHaveValidationErrorFor(c => c.UserId);
            else
                result.ShouldHaveValidationErrorFor(c => c.UserId);
        }

        [TestCase("", false)]
        [TestCase(null, true)]
        [TestCase(" ", false)]
        [TestCase("DATA", false)]
        [TestCase("d@b.c", true)]
        public void Validate_ContactUsEmail(string email, bool isValid)
        {
            var validator = new UpdateProviderCourseCommandValidator();

            var result = validator.TestValidate(new UpdateProviderCourseCommand { ContactUsEmail = email });

            if (isValid)
                result.ShouldNotHaveValidationErrorFor(c => c.ContactUsEmail);
            else
                result.ShouldHaveValidationErrorFor(c => c.ContactUsEmail);
        }

        [Test]
        public void Validate_ContactUsEmailTooLong_FailsValidation()
        {
            var email = new string('a', 250) + "@aa.com";
            var validator = new UpdateProviderCourseCommandValidator();

            var result = validator.TestValidate(new UpdateProviderCourseCommand { ContactUsEmail = email });
            result.ShouldHaveValidationErrorFor(c => c.ContactUsEmail);
        }

        [TestCase(null, true)]
        [TestCase("DATA", false)]
        [TestCase("1234567890", true)]
        public void Validate_ContactUsPhoneNumber(string phone, bool isValid)
        {
            var validator = new UpdateProviderCourseCommandValidator();

            var result = validator.TestValidate(new UpdateProviderCourseCommand { ContactUsPhoneNumber = phone });

            if (isValid)
                result.ShouldNotHaveValidationErrorFor(c => c.ContactUsPhoneNumber);
            else
                result.ShouldHaveValidationErrorFor(c => c.ContactUsPhoneNumber);
        }

        [Test]
        public void Validate_ContactUsPhoneNumberTooLong_FailsValidation()
        {
            var phone = new string('a', 51);
            var validator = new UpdateProviderCourseCommandValidator();

            var result = validator.TestValidate(new UpdateProviderCourseCommand { ContactUsPhoneNumber = phone });
            result.ShouldHaveValidationErrorFor(c => c.ContactUsPhoneNumber);
        }

        [TestCase(null, true)]
        [TestCase("DATA", false)]
        [TestCase("www.s.ad", true)]
        public void Validate_ContactUsPageUrl(string url, bool isValid)
        {
            var validator = new UpdateProviderCourseCommandValidator();

            var result = validator.TestValidate(new UpdateProviderCourseCommand { ContactUsPageUrl = url });

            if (isValid)
                result.ShouldNotHaveValidationErrorFor(c => c.ContactUsPageUrl);
            else
                result.ShouldHaveValidationErrorFor(c => c.ContactUsPageUrl);
        }

        [Test]
        public void Validate_ContactUsPageUrlTooLong_FailsValidation()
        {
            var url = new string('a', 497) + ".net";
            var validator = new UpdateProviderCourseCommandValidator();

            var result = validator.TestValidate(new UpdateProviderCourseCommand { ContactUsPageUrl = url });
            result.ShouldHaveValidationErrorFor(c => c.ContactUsPageUrl);
        }

        [TestCase(null, true)]
        [TestCase("DATA", false)]
        [TestCase("www.s.ad", true)]
        public void Validate_StandardInfoUrl(string url, bool isValid)
        {
            var validator = new UpdateProviderCourseCommandValidator();

            var result = validator.TestValidate(new UpdateProviderCourseCommand { StandardInfoUrl = url });

            if (isValid)
                result.ShouldNotHaveValidationErrorFor(c => c.StandardInfoUrl);
            else
                result.ShouldHaveValidationErrorFor(c => c.StandardInfoUrl);
        }

        [Test]
        public void Validate_StandardInfoUrlTooLong_FailsValidation()
        {
            var url = new string('a', 497) + ".net";
            var validator = new UpdateProviderCourseCommandValidator();

            var result = validator.TestValidate(new UpdateProviderCourseCommand { StandardInfoUrl = url });
            result.ShouldHaveValidationErrorFor(c => c.StandardInfoUrl);
        }
    }
}
