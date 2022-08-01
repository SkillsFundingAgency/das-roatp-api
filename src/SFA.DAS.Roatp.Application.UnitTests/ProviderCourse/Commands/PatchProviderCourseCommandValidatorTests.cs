using System.Linq;
using System.Threading.Tasks;
using FluentValidation.TestHelper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.ProviderCourse;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Roatp.Domain.Models;

namespace SFA.DAS.Roatp.Application.UnitTests.ProviderCourse.Commands
{
    [TestFixture]
    public class PatchProviderCourseCommandValidatorTests
    {
        private Mock<IProviderReadRepository> _providerReadRepo;
        private Mock<IProviderCourseReadRepository> _providerCourseReadRepo;

        private const string IsApprovedByRegulator = "IsApprovedByRegulator";
        private const string ContactUsEmail = "ContactUsEmail";
        private const string ContactUsPhoneNumber = "ContactUsPhoneNumber";
        private const string ContactUsPageUrl = "ContactUsPageUrl";
        private const string StandardInfoUrl = "StandardInfoUrl";

        private const string Replace = "Replace";

        [SetUp]
        public void Before_each_test()
        {
            _providerReadRepo = new Mock<IProviderReadRepository>();
            _providerCourseReadRepo = new Mock<IProviderCourseReadRepository>();
            _providerReadRepo.Setup(x => x.GetByUkprn(It.IsAny<int>())).ReturnsAsync(new Provider());
            _providerCourseReadRepo.Setup(x => x.GetProviderCourse(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(new Domain.Entities.ProviderCourse());

        }

        [TestCase(10000000, false)]
        [TestCase(10000001, true)]
        [TestCase(100000000, false)]
        public async Task Validate_Ukprn(int ukprn, bool isValid)
        {
            var validator = new PatchProviderCourseCommandValidator(_providerReadRepo.Object, _providerCourseReadRepo.Object);

             var result =  await validator.TestValidateAsync(new PatchProviderCourseCommand { Ukprn = ukprn, Patch = new JsonPatchDocument<PatchProviderCourse>()});
            
            if (isValid)
                result.ShouldNotHaveValidationErrorFor(c => c.Ukprn);
            else
                result.ShouldHaveValidationErrorFor(c => c.Ukprn);
        }

        [TestCase(0, false)]
        [TestCase(1, true)]
        [TestCase(-1, false)]
        public async Task Validate_LarsCode(int larsCode, bool isValid)
        {
            var validator = new PatchProviderCourseCommandValidator(_providerReadRepo.Object, _providerCourseReadRepo.Object);

            var result = await validator.TestValidateAsync(new PatchProviderCourseCommand { LarsCode = larsCode, Patch = new JsonPatchDocument<PatchProviderCourse>()});
        
            if (isValid)
                result.ShouldNotHaveValidationErrorFor(c => c.LarsCode);
            else
                result.ShouldHaveValidationErrorFor(c => c.LarsCode);
        }

        [Test]
        public async Task Validate_Patch_IsApprovedByRegulator_MatchingOperations_NoErrorMessage()
        {
            var validator = new PatchProviderCourseCommandValidator(_providerReadRepo.Object, _providerCourseReadRepo.Object);
            var ukprn = 10000001;
            var larsCode = 1;

            var command = new PatchProviderCourseCommand
            {
                Ukprn = ukprn,
                LarsCode = larsCode,
                Patch = new JsonPatchDocument<PatchProviderCourse>()
            };

            command.Patch = new JsonPatchDocument<PatchProviderCourse>
            {
                Operations =
                {
                    new Operation<PatchProviderCourse>
                        { op = Replace, path = IsApprovedByRegulator, value = "True" }
                }
            };

            var result = await validator.TestValidateAsync(command);

            result.ShouldNotHaveAnyValidationErrors();
            Assert.IsTrue(result.IsValid);
        }

        [TestCase("True",true)]
        [TestCase("False", true)]
        [TestCase("true", true)]
        [TestCase("false", true)]
        [TestCase("not boolean", false)]
        [TestCase("2", false)]
        public async Task Validate_Patch_IsApprovedByRegulator_VariousFieldValues_MatchingErrors(string isApprovedByRegulatorValue, bool isNoErrorExpected)
        {
            var validator = new PatchProviderCourseCommandValidator(_providerReadRepo.Object, _providerCourseReadRepo.Object);
            var ukprn = 10000001;
            var larsCode = 1;

            var command = new PatchProviderCourseCommand
            {
                Ukprn = ukprn,
                LarsCode = larsCode,
                Patch = new JsonPatchDocument<PatchProviderCourse>()
            };

            command.Patch = new JsonPatchDocument<PatchProviderCourse>
            {
                Operations =
                {
                    new Operation<PatchProviderCourse>
                        { op = Replace, path = IsApprovedByRegulator, value = isApprovedByRegulatorValue }
                }
            };

            var result = await validator.TestValidateAsync(command);
            if (isNoErrorExpected)
            {
                result.ShouldNotHaveAnyValidationErrors();
                Assert.IsTrue(result.IsValid);
            }
            else
            {
                Assert.IsFalse(result.IsValid);
                Assert.IsTrue(result.Errors.Count == 1);
                Assert.AreEqual(PatchProviderCourseCommandValidator.IsApprovedByRegulatorIsNotABooleanErrorMessage, result.Errors[0].ErrorMessage);
            }
        }

        [Test]
        public async Task Validate_Patch_ContactDetails_MatchingOperations_NoErrorMessage()
        {
            var validator = new PatchProviderCourseCommandValidator(_providerReadRepo.Object, _providerCourseReadRepo.Object);
            var ukprn = 10000001;
            var larsCode = 1;

            var command = new PatchProviderCourseCommand
            {
                Ukprn = ukprn,
                LarsCode = larsCode,
                Patch = new JsonPatchDocument<PatchProviderCourse>()
            };

            command.Patch = new JsonPatchDocument<PatchProviderCourse>
            {
                Operations =
                {
                    new Operation<PatchProviderCourse>
                        { op = Replace, path = StandardInfoUrl, value = "http://www.test.com" },
                    new Operation<PatchProviderCourse>
                        { op = Replace, path = ContactUsPhoneNumber, value = "1234567890" },
                    new Operation<PatchProviderCourse>
                        { op = Replace, path = ContactUsEmail, value = "test@test.com" },
                    new Operation<PatchProviderCourse>
                        { op = Replace, path = ContactUsPageUrl, value = "http://www.test.com/contact-us" },
                    new Operation<PatchProviderCourse>
                        { op = Replace, path = IsApprovedByRegulator, value = "True" }
                }
            };

            var result = await validator.TestValidateAsync(command);

            result.ShouldNotHaveAnyValidationErrors();
            Assert.IsTrue(result.IsValid);
        }

        [Test]
        public async Task Validate_Patch_ContactDetails_MatchingOperationsWithUnavailableFieldOperation_UnavailableFieldErrorMessage()
        {
            var validator = new PatchProviderCourseCommandValidator(_providerReadRepo.Object, _providerCourseReadRepo.Object);
            var ukprn = 10000001;
            var larsCode = 1;

            var command = new PatchProviderCourseCommand
            {
                Ukprn = ukprn,
                LarsCode = larsCode,
                Patch = new JsonPatchDocument<PatchProviderCourse>()
            };

            command.Patch = new JsonPatchDocument<PatchProviderCourse>
            {
                Operations =
                {
                    new Operation<PatchProviderCourse>
                        { op = Replace, path = StandardInfoUrl, value = "http://www.test.com" },
                    new Operation<PatchProviderCourse>
                        { op = Replace, path = ContactUsPhoneNumber, value = "1234567890" },
                    new Operation<PatchProviderCourse>
                        { op = Replace, path = ContactUsEmail, value = "test@test.com" },
                    new Operation<PatchProviderCourse>
                        { op = Replace, path = ContactUsPageUrl, value = "http://www.test.com/contact-us" },
                    new Operation<PatchProviderCourse>
                        { op = Replace, path = IsApprovedByRegulator, value = "True" },
                    new Operation<PatchProviderCourse>
                        { op = Replace, path = "unexpectedField", value = "field" }
                }
            };
            
            var result = await validator.TestValidateAsync(command);

            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.Errors.Count == 1);
            Assert.AreEqual(PatchProviderCourseCommandValidator.PatchOperationContainsUnavailableFieldErrorMessage, result.Errors[0].ErrorMessage);
        }

        [Test]
        public async Task Validate_Patch_NoOperations_ErrorMessage()
        {
            var validator = new PatchProviderCourseCommandValidator(_providerReadRepo.Object, _providerCourseReadRepo.Object);
            var ukprn = 10000001;
            var larsCode = 1;

            var command = new PatchProviderCourseCommand
            {
                Ukprn = ukprn,
                LarsCode = larsCode,
                Patch = new JsonPatchDocument<PatchProviderCourse>()
            };

            var result = await validator.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(c => c.Patch.Operations.Count);
            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.Errors.Count == 1);
            Assert.AreEqual(PatchProviderCourseCommandValidator.NoPatchOperationsPresentErrorMessage, result.Errors[0].ErrorMessage);
        }

        [Test]
        public async Task Validate_Patch_ContactDetails_InvalidEmailFormat_ExpectedErrorMessage()
        {
            var validator = new PatchProviderCourseCommandValidator(_providerReadRepo.Object, _providerCourseReadRepo.Object);
            var ukprn = 10000001;
            var larsCode = 1;

            var command = new PatchProviderCourseCommand
            {
                Ukprn = ukprn,
                LarsCode = larsCode,
                Patch = new JsonPatchDocument<PatchProviderCourse>()
            };

            command.Patch = new JsonPatchDocument<PatchProviderCourse>
            {
                Operations =
                {
                    new Operation<PatchProviderCourse>
                        { op = Replace, path = StandardInfoUrl, value = "http://www.test.com" },
                    new Operation<PatchProviderCourse>
                        { op = Replace, path = ContactUsPhoneNumber, value = "1234567890" },
                    new Operation<PatchProviderCourse>
                        { op = Replace, path = ContactUsEmail, value = "invalidEmail" },
                    new Operation<PatchProviderCourse>
                        { op = Replace, path = ContactUsPageUrl, value = "http://www.test.com/contact-us" }
                }
            };

            var result = await validator.TestValidateAsync(command);

            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.Errors.Count == 1);
            Assert.AreEqual(PatchProviderCourseCommandValidator.EmailAddressWrongFormat, result.Errors[0].ErrorMessage);
        }

        [Test]
        public async Task Validate_Patch_ContactDetails_EmailAddressTooLong_ExpectedErrorMessage()
        {
            var validator = new PatchProviderCourseCommandValidator(_providerReadRepo.Object, _providerCourseReadRepo.Object);
            var ukprn = 10000001;
            var larsCode = 1;

            var command = new PatchProviderCourseCommand
            {
                Ukprn = ukprn,
                LarsCode = larsCode,
                Patch = new JsonPatchDocument<PatchProviderCourse>()
            };

            command.Patch = new JsonPatchDocument<PatchProviderCourse>
            {
                Operations =
                {
                    new Operation<PatchProviderCourse>
                        { op = Replace, path = StandardInfoUrl, value = "http://www.test.com" },
                    new Operation<PatchProviderCourse>
                        { op = Replace, path = ContactUsPhoneNumber, value = "1234567890" },
                    new Operation<PatchProviderCourse>
                        { op = Replace, path = ContactUsEmail, value = new string('x',255) + "@test.com" },
                    new Operation<PatchProviderCourse>
                        { op = Replace, path = ContactUsPageUrl, value = "http://www.test.com/contact-us" }
                }
            };

            var result = await validator.TestValidateAsync(command);

            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.Errors.Count == 1);
            Assert.AreEqual(PatchProviderCourseCommandValidator.EmailAddressTooLong, result.Errors[0].ErrorMessage);
        }

        [Test]
        public async Task Validate_Patch_ContactDetails_EmailAddressWrongFormatAndTooLong()
        {
            var validator = new PatchProviderCourseCommandValidator(_providerReadRepo.Object, _providerCourseReadRepo.Object);
            var ukprn = 10000001;
            var larsCode = 1;

            var command = new PatchProviderCourseCommand
            {
                Ukprn = ukprn,
                LarsCode = larsCode,
                Patch = new JsonPatchDocument<PatchProviderCourse>()
            };

            command.Patch = new JsonPatchDocument<PatchProviderCourse>
            {
                Operations =
                {
                    new Operation<PatchProviderCourse>
                        { op = Replace, path = StandardInfoUrl, value = "http://www.test.com" },
                    new Operation<PatchProviderCourse>
                        { op = Replace, path = ContactUsPhoneNumber, value = "1234567890" },
                    new Operation<PatchProviderCourse>
                        { op = Replace, path = ContactUsEmail, value = new string('x',260)},
                    new Operation<PatchProviderCourse>
                        { op = Replace, path = ContactUsPageUrl, value = "http://www.test.com/contact-us" }
                }
            };

            var result = await validator.TestValidateAsync(command);

            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.Errors.Count == 2);
            Assert.IsTrue(result.Errors.Any(x => x.ErrorMessage==PatchProviderCourseCommandValidator.EmailAddressTooLong));
            Assert.IsTrue(result.Errors.Any(x => x.ErrorMessage == PatchProviderCourseCommandValidator.EmailAddressWrongFormat));
        }

        [TestCase(1,true)]
        [TestCase(2, true)]
        [TestCase(3, true)]
        [TestCase(4, true)]
        [TestCase(5, true)]
        [TestCase(6, true)]
        [TestCase(7, true)]
        [TestCase(8, true)]
        [TestCase(9, true)]
        [TestCase(10, false)]
        [TestCase(20, false)]
        [TestCase(30, false)]
        [TestCase(40, false)]
        [TestCase(50, false)]
        [TestCase(51, true)]
        [TestCase(100, true)]
        public async Task Validate_Patch_ContactDetails_ContactPhoneNumber_ExpectedErrorStatus(int phoneNumberLength, bool isErrorExpected)
        {
            var validator = new PatchProviderCourseCommandValidator(_providerReadRepo.Object, _providerCourseReadRepo.Object);
            var ukprn = 10000001;
            var larsCode = 1;

            var command = new PatchProviderCourseCommand
            {
                Ukprn = ukprn,
                LarsCode = larsCode,
                Patch = new JsonPatchDocument<PatchProviderCourse>()
            };

            command.Patch = new JsonPatchDocument<PatchProviderCourse>
            {
                Operations =
                {
                    new Operation<PatchProviderCourse>
                        { op = Replace, path = StandardInfoUrl, value = "http://www.test.com" },
                    new Operation<PatchProviderCourse>
                        { op = Replace, path = ContactUsPhoneNumber, value = new string('1',phoneNumberLength) },
                    new Operation<PatchProviderCourse>
                        { op = Replace, path = ContactUsEmail, value = "test@test.com" },
                    new Operation<PatchProviderCourse>
                        { op = Replace, path = ContactUsPageUrl, value = "http://www.test.com/contact-us" }
                }
            };

            var result = await validator.TestValidateAsync(command);

            Assert.IsTrue(result.IsValid == !isErrorExpected);
            if (isErrorExpected)
            {
                Assert.IsTrue(result.Errors.Count == 1);
                Assert.AreEqual(PatchProviderCourseCommandValidator.PhoneNumberWrongLength,
                    result.Errors[0].ErrorMessage);
            }
            else
            {
                Assert.IsTrue(!result.Errors.Any());
            }
        }

        [Test]
        public async Task Validate_Patch_ContactDetails_ContactUrlWrongFormat_ExpectedErrorMessage()
        {
            var validator = new PatchProviderCourseCommandValidator(_providerReadRepo.Object, _providerCourseReadRepo.Object);
            var ukprn = 10000001;
            var larsCode = 1;

            var command = new PatchProviderCourseCommand
            {
                Ukprn = ukprn,
                LarsCode = larsCode,
                Patch = new JsonPatchDocument<PatchProviderCourse>()
            };

            command.Patch = new JsonPatchDocument<PatchProviderCourse>
            {
                Operations =
                {
                    new Operation<PatchProviderCourse>
                        { op = Replace, path = StandardInfoUrl, value = "http://www.test.com" },
                    new Operation<PatchProviderCourse>
                        { op = Replace, path = ContactUsPhoneNumber, value = "1234567890" },
                    new Operation<PatchProviderCourse>
                        { op = Replace, path = ContactUsEmail, value = "test@test.com" },
                    new Operation<PatchProviderCourse>
                        { op = Replace, path = ContactUsPageUrl, value = "wrongformat" }
                }
            };

            var result = await validator.TestValidateAsync(command);

            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.Errors.Count == 1);
            Assert.AreEqual(PatchProviderCourseCommandValidator.ContactUsPageUrlWrongFormat, result.Errors[0].ErrorMessage);
        }

        [Test]
        public async Task Validate_Patch_ContactDetails_ContactUrlTooLong_ExpectedErrorMessage()
        {
            var validator = new PatchProviderCourseCommandValidator(_providerReadRepo.Object, _providerCourseReadRepo.Object);
            var ukprn = 10000001;
            var larsCode = 1;

            var command = new PatchProviderCourseCommand
            {
                Ukprn = ukprn,
                LarsCode = larsCode,
                Patch = new JsonPatchDocument<PatchProviderCourse>()
            };

            command.Patch = new JsonPatchDocument<PatchProviderCourse>
            {
                Operations =
                {
                    new Operation<PatchProviderCourse>
                        { op = Replace, path = StandardInfoUrl, value = "http://www.test.com" },
                    new Operation<PatchProviderCourse>
                        { op = Replace, path = ContactUsPhoneNumber, value = "1234567890" },
                    new Operation<PatchProviderCourse>
                        { op = Replace, path = ContactUsEmail, value="test@test.com"},
                    new Operation<PatchProviderCourse>
                        { op = Replace, path = ContactUsPageUrl,  value = new string('x',497) + ".com" }
                }
            };

            var result = await validator.TestValidateAsync(command);

            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.Errors.Count == 1);
            Assert.AreEqual(PatchProviderCourseCommandValidator.ContactUsPageUrlTooLong, result.Errors[0].ErrorMessage);
        }
    }
}

