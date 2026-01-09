using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using FluentValidation.TestHelper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.ProviderCourse.Commands.PatchProviderCourse;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using static SFA.DAS.Roatp.Application.Common.ValidationMessages;

namespace SFA.DAS.Roatp.Application.UnitTests.ProviderCourse.Commands.PatchProviderCourse;

[TestFixture]
public class PatchProviderCourseCommandValidatorTests
{
    private Mock<IProvidersReadRepository> _providersReadRepo;
    private Mock<IProviderCoursesReadRepository> _providerCoursesReadRepo;

    private const string IsApprovedByRegulator = "IsApprovedByRegulator";
    private const string ContactUsEmail = "ContactUsEmail";
    private const string ContactUsPhoneNumber = "ContactUsPhoneNumber";
    private const string StandardInfoUrl = "StandardInfoUrl";
    private const string HasOnlineDeliveryOption = "HasOnlineDeliveryOption";

    private const string Replace = "replace";

    [SetUp]
    public void Before_each_test()
    {
        _providersReadRepo = new Mock<IProvidersReadRepository>();
        _providerCoursesReadRepo = new Mock<IProviderCoursesReadRepository>();
        _providersReadRepo.Setup(x => x.GetByUkprn(It.IsAny<int>())).ReturnsAsync(new Provider());
        _providerCoursesReadRepo.Setup(x => x.GetProviderCourse(It.IsAny<int>(), It.IsAny<string>())).ReturnsAsync(new Domain.Entities.ProviderCourse());

    }

    [TestCase(10000000, false)]
    [TestCase(10000001, true)]
    [TestCase(100000000, false)]
    public async Task Validate_Ukprn(int ukprn, bool isValid)
    {
        var validator = new PatchProviderCourseCommandValidator(_providersReadRepo.Object, _providerCoursesReadRepo.Object);

        var result = await validator.TestValidateAsync(new PatchProviderCourseCommand { Ukprn = ukprn, Patch = new JsonPatchDocument<Domain.Models.PatchProviderCourse>() });

        if (isValid)
            result.ShouldNotHaveValidationErrorFor(c => c.Ukprn);
        else
            result.ShouldHaveValidationErrorFor(c => c.Ukprn);
    }

    [TestCase("", false)]
    [TestCase("1", true)]
    public async Task Validate_LarsCode(string larsCode, bool isValid)
    {
        var validator = new PatchProviderCourseCommandValidator(_providersReadRepo.Object, _providerCoursesReadRepo.Object);

        var result = await validator.TestValidateAsync(new PatchProviderCourseCommand { LarsCode = larsCode, Patch = new JsonPatchDocument<Domain.Models.PatchProviderCourse>() });

        if (isValid)
            result.ShouldNotHaveValidationErrorFor(c => c.LarsCode);
        else
            result.ShouldHaveValidationErrorFor(c => c.LarsCode);
    }

    [Test]
    public async Task Validate_Patch_IsApprovedByRegulator_MatchingOperations_NoErrorMessage()
    {
        var validator = new PatchProviderCourseCommandValidator(_providersReadRepo.Object, _providerCoursesReadRepo.Object);
        var ukprn = 10000001;
        var larsCode = "1";

        var command = new PatchProviderCourseCommand
        {
            Ukprn = ukprn,
            LarsCode = larsCode,
            Patch = new JsonPatchDocument<Domain.Models.PatchProviderCourse>()
        };

        command.Patch = new JsonPatchDocument<Domain.Models.PatchProviderCourse>
        {
            Operations =
            {
                new Operation<Domain.Models.PatchProviderCourse>
                    { op = Replace, path = IsApprovedByRegulator, value = "True" }
            }
        };

        var result = await validator.TestValidateAsync(command);

        result.ShouldNotHaveAnyValidationErrors();
        result.IsValid.Should().BeTrue();
    }

    [TestCase("True", true)]
    [TestCase("False", true)]
    [TestCase("true", true)]
    [TestCase("false", true)]
    [TestCase("not boolean", false)]
    [TestCase("2", false)]
    public async Task Validate_Patch_IsApprovedByRegulator_VariousFieldValues_MatchingErrors(string isApprovedByRegulatorValue, bool isNoErrorExpected)
    {
        var validator = new PatchProviderCourseCommandValidator(_providersReadRepo.Object, _providerCoursesReadRepo.Object);
        var ukprn = 10000001;
        var larsCode = "1";

        var command = new PatchProviderCourseCommand
        {
            Ukprn = ukprn,
            LarsCode = larsCode,
            Patch = new JsonPatchDocument<Domain.Models.PatchProviderCourse>()
        };

        command.Patch = new JsonPatchDocument<Domain.Models.PatchProviderCourse>
        {
            Operations =
            {
                new Operation<Domain.Models.PatchProviderCourse>
                    { op = Replace, path = IsApprovedByRegulator, value = isApprovedByRegulatorValue }
            }
        };

        var result = await validator.TestValidateAsync(command);
        if (isNoErrorExpected)
        {
            result.ShouldNotHaveAnyValidationErrors();
            result.IsValid.Should().BeTrue();
        }
        else
        {
            result.IsValid.Should().BeFalse();
            result.Errors.Should().HaveCount(1);
            PatchProviderCourseCommandValidator.IsApprovedByRegulatorIsNotABooleanErrorMessage.Should().Be(result.Errors[0].ErrorMessage);
        }
    }

    [Test]
    public async Task Validate_Patch_ContactDetails_MatchingOperations_NoErrorMessage()
    {
        var validator = new PatchProviderCourseCommandValidator(_providersReadRepo.Object, _providerCoursesReadRepo.Object);
        var ukprn = 10000001;
        var larsCode = "1";

        var command = new PatchProviderCourseCommand
        {
            Ukprn = ukprn,
            LarsCode = larsCode,
            Patch = new JsonPatchDocument<Domain.Models.PatchProviderCourse>()
        };

        command.Patch = new JsonPatchDocument<Domain.Models.PatchProviderCourse>
        {
            Operations =
            {
                new Operation<Domain.Models.PatchProviderCourse>
                    { op = Replace, path = StandardInfoUrl, value = "http://www.test.com" },
                new Operation<Domain.Models.PatchProviderCourse>
                    { op = Replace, path = ContactUsPhoneNumber, value = "1234567890" },
                new Operation<Domain.Models.PatchProviderCourse>
                    { op = Replace, path = ContactUsEmail, value = "test@test.com" },
                new Operation<Domain.Models.PatchProviderCourse>
                    { op = Replace, path = IsApprovedByRegulator, value = "True" }
            }
        };

        var result = await validator.TestValidateAsync(command);

        result.ShouldNotHaveAnyValidationErrors();
        result.IsValid.Should().BeTrue();
    }

    [Test]
    public async Task Validate_Patch_ContactDetails_MatchingOperationsWithUnavailableFieldOperation_UnavailableFieldErrorMessage()
    {
        var validator = new PatchProviderCourseCommandValidator(_providersReadRepo.Object, _providerCoursesReadRepo.Object);
        var ukprn = 10000001;
        var larsCode = "1";

        var command = new PatchProviderCourseCommand
        {
            Ukprn = ukprn,
            LarsCode = larsCode,
            Patch = new JsonPatchDocument<Domain.Models.PatchProviderCourse>()
        };

        command.Patch = new JsonPatchDocument<Domain.Models.PatchProviderCourse>
        {
            Operations =
            {
                new Operation<Domain.Models.PatchProviderCourse>
                    { op = Replace, path = StandardInfoUrl, value = "http://www.test.com" },
                new Operation<Domain.Models.PatchProviderCourse>
                    { op = Replace, path = ContactUsPhoneNumber, value = "1234567890" },
                new Operation<Domain.Models.PatchProviderCourse>
                    { op = Replace, path = ContactUsEmail, value = "test@test.com" },
                new Operation<Domain.Models.PatchProviderCourse>
                    { op = Replace, path = IsApprovedByRegulator, value = "True" },
                new Operation<Domain.Models.PatchProviderCourse>
                    { op = Replace, path = "unexpectedField", value = "field" }
            }
        };

        var result = await validator.TestValidateAsync(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCount(1);
        PatchProviderCourseCommandValidator.PatchOperationContainsUnavailableFieldErrorMessage.Should().Be(result.Errors[0].ErrorMessage);
    }

    [Test]
    public async Task Validate_Patch_ContactDetails_MatchingOperationsWithUnavailableOperation_UnavailableFieldErrorMessage()
    {
        var validator = new PatchProviderCourseCommandValidator(_providersReadRepo.Object, _providerCoursesReadRepo.Object);
        var ukprn = 10000001;
        var larsCode = "1";

        var command = new PatchProviderCourseCommand
        {
            Ukprn = ukprn,
            LarsCode = larsCode,
            Patch = new JsonPatchDocument<Domain.Models.PatchProviderCourse>()
        };

        command.Patch = new JsonPatchDocument<Domain.Models.PatchProviderCourse>
        {
            Operations =
            {
                new Operation<Domain.Models.PatchProviderCourse>
                    { op = "Add", path = StandardInfoUrl, value = "http://www.test.com" },
                new Operation<Domain.Models.PatchProviderCourse>
                    { op = Replace, path = ContactUsPhoneNumber, value = "1234567890" },
                new Operation<Domain.Models.PatchProviderCourse>
                    { op = Replace, path = ContactUsEmail, value = "test@test.com" },
                new Operation<Domain.Models.PatchProviderCourse>
                    { op = Replace, path = IsApprovedByRegulator, value = "True" }
            }
        };

        var result = await validator.TestValidateAsync(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCount(1);
        PatchProviderCourseCommandValidator.PatchOperationContainsUnavailableOperationErrorMessage.Should().Be(result.Errors[0].ErrorMessage);
    }


    [Test]
    public async Task Validate_Patch_NoOperations_ErrorMessage()
    {
        var validator = new PatchProviderCourseCommandValidator(_providersReadRepo.Object, _providerCoursesReadRepo.Object);
        var ukprn = 10000001;
        var larsCode = "1";

        var command = new PatchProviderCourseCommand
        {
            Ukprn = ukprn,
            LarsCode = larsCode,
            Patch = new JsonPatchDocument<Domain.Models.PatchProviderCourse>()
        };

        var result = await validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(c => c.Patch.Operations.Count);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCount(1);
        PatchProviderCourseCommandValidator.NoPatchOperationsPresentErrorMessage.Should().Be(result.Errors[0].ErrorMessage);
    }

    [Test]
    public async Task Validate_Patch_ContactDetails_InvalidEmailFormat_ExpectedErrorMessage()
    {
        var validator = new PatchProviderCourseCommandValidator(_providersReadRepo.Object, _providerCoursesReadRepo.Object);
        var ukprn = 10000001;
        var larsCode = "1";

        var command = new PatchProviderCourseCommand
        {
            Ukprn = ukprn,
            LarsCode = larsCode,
            Patch = new JsonPatchDocument<Domain.Models.PatchProviderCourse>()
        };

        command.Patch = new JsonPatchDocument<Domain.Models.PatchProviderCourse>
        {
            Operations =
            {
                new Operation<Domain.Models.PatchProviderCourse>
                    { op = Replace, path = StandardInfoUrl, value = "http://www.test.com" },
                new Operation<Domain.Models.PatchProviderCourse>
                    { op = Replace, path = ContactUsPhoneNumber, value = "1234567890" },
                new Operation<Domain.Models.PatchProviderCourse>
                    { op = Replace, path = ContactUsEmail, value = "invalidEmail" }
            }
        };

        var result = await validator.TestValidateAsync(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCount(1);
        EmailValidationMessages.EmailAddressWrongFormat.Should().Be(result.Errors[0].ErrorMessage);
    }

    [Test]
    public async Task Validate_Patch_ContactDetails_EmailAddressTooLong_ExpectedErrorMessage()
    {
        var validator = new PatchProviderCourseCommandValidator(_providersReadRepo.Object, _providerCoursesReadRepo.Object);
        var ukprn = 10000001;
        var larsCode = "1";

        var command = new PatchProviderCourseCommand
        {
            Ukprn = ukprn,
            LarsCode = larsCode,
            Patch = new JsonPatchDocument<Domain.Models.PatchProviderCourse>()
        };

        command.Patch = new JsonPatchDocument<Domain.Models.PatchProviderCourse>
        {
            Operations =
            {
                new Operation<Domain.Models.PatchProviderCourse>
                    { op = Replace, path = StandardInfoUrl, value = "http://www.test.com" },
                new Operation<Domain.Models.PatchProviderCourse>
                    { op = Replace, path = ContactUsPhoneNumber, value = "1234567890" },
                new Operation<Domain.Models.PatchProviderCourse>
                    { op = Replace, path = ContactUsEmail, value = new string('x',255) + "@test.com" }
            }
        };

        var result = await validator.TestValidateAsync(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCount(1);
        EmailValidationMessages.EmailAddressTooLong.Should().Be(result.Errors[0].ErrorMessage);
    }

    [Test]
    public async Task Validate_Patch_ContactDetails_EmailAddressWrongFormatAndTooLong()
    {
        var validator = new PatchProviderCourseCommandValidator(_providersReadRepo.Object, _providerCoursesReadRepo.Object);
        var ukprn = 10000001;
        var larsCode = "1";

        var command = new PatchProviderCourseCommand
        {
            Ukprn = ukprn,
            LarsCode = larsCode,
            Patch = new JsonPatchDocument<Domain.Models.PatchProviderCourse>()
        };

        command.Patch = new JsonPatchDocument<Domain.Models.PatchProviderCourse>
        {
            Operations =
            {
                new Operation<Domain.Models.PatchProviderCourse>
                    { op = Replace, path = StandardInfoUrl, value = "http://www.test.com" },
                new Operation<Domain.Models.PatchProviderCourse>
                    { op = Replace, path = ContactUsPhoneNumber, value = "1234567890" },
                new Operation<Domain.Models.PatchProviderCourse>
                    { op = Replace, path = ContactUsEmail, value = new string('x',260)},
            }
        };

        var result = await validator.TestValidateAsync(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCount(2);
        result.Errors.Any(x => x.ErrorMessage == EmailValidationMessages.EmailAddressTooLong).Should().BeTrue();
        result.Errors.Any(x => x.ErrorMessage == EmailValidationMessages.EmailAddressWrongFormat).Should().BeTrue();
    }

    [TestCase(1, true)]
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
        var validator = new PatchProviderCourseCommandValidator(_providersReadRepo.Object, _providerCoursesReadRepo.Object);
        var ukprn = 10000001;
        var larsCode = "1";

        var command = new PatchProviderCourseCommand
        {
            Ukprn = ukprn,
            LarsCode = larsCode,
            Patch = new JsonPatchDocument<Domain.Models.PatchProviderCourse>()
        };

        command.Patch = new JsonPatchDocument<Domain.Models.PatchProviderCourse>
        {
            Operations =
            {
                new Operation<Domain.Models.PatchProviderCourse>
                    { op = Replace, path = StandardInfoUrl, value = "http://www.test.com" },
                new Operation<Domain.Models.PatchProviderCourse>
                    { op = Replace, path = ContactUsPhoneNumber, value = new string('1',phoneNumberLength) },
                new Operation<Domain.Models.PatchProviderCourse>
                    { op = Replace, path = ContactUsEmail, value = "test@test.com" }
            }
        };

        var result = await validator.TestValidateAsync(command);

        result.IsValid.Should().Be(!isErrorExpected);
        if (isErrorExpected)
        {
            result.Errors.Should().HaveCount(1);
            PhoneNumberValidationMessages.PhoneNumberWrongLength.Should().Be(result.Errors[0].ErrorMessage);
        }
        else
        {
            result.Errors.Should().BeEmpty();
        }
    }

    [Test]
    public async Task Validate_Patch_ContactDetails_ContactUrlWrongFormat_ExpectedErrorMessage()
    {
        var validator = new PatchProviderCourseCommandValidator(_providersReadRepo.Object, _providerCoursesReadRepo.Object);
        var ukprn = 10000001;
        var larsCode = "1";

        var command = new PatchProviderCourseCommand
        {
            Ukprn = ukprn,
            LarsCode = larsCode,
            Patch = new JsonPatchDocument<Domain.Models.PatchProviderCourse>()
        };

        command.Patch = new JsonPatchDocument<Domain.Models.PatchProviderCourse>
        {
            Operations =
            {
                new Operation<Domain.Models.PatchProviderCourse>
                    { op = Replace, path = StandardInfoUrl, value = "wrongformat" },
                new Operation<Domain.Models.PatchProviderCourse>
                    { op = Replace, path = ContactUsPhoneNumber, value = "1234567890" },
                new Operation<Domain.Models.PatchProviderCourse>
                    { op = Replace, path = ContactUsEmail, value = "test@test.com" }
            }
        };

        var result = await validator.TestValidateAsync(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCount(1);
        result.ShouldHaveValidationErrorFor(c => c.StandardInfoUrl).WithErrorMessage(UrlValidationMessages.UrlWrongFormat("Website"));
    }

    [Test]
    public async Task Validate_Patch_HasOnlineDeliveryOption_MatchingOperations_NoErrorMessage()
    {
        var validator = new PatchProviderCourseCommandValidator(_providersReadRepo.Object, _providerCoursesReadRepo.Object);
        var ukprn = 10000001;
        var larsCode = "1";

        var command = new PatchProviderCourseCommand
        {
            Ukprn = ukprn,
            LarsCode = larsCode,
            Patch = new JsonPatchDocument<Domain.Models.PatchProviderCourse>()
        };

        command.Patch = new JsonPatchDocument<Domain.Models.PatchProviderCourse>
        {
            Operations =
            {
                new Operation<Domain.Models.PatchProviderCourse>
                    { op = Replace, path = HasOnlineDeliveryOption, value = "True" }
            }
        };

        var result = await validator.TestValidateAsync(command);

        result.ShouldNotHaveAnyValidationErrors();
        result.IsValid.Should().BeTrue();
    }

    [TestCase("True", true)]
    [TestCase("False", true)]
    [TestCase("true", true)]
    [TestCase("false", true)]
    [TestCase("not boolean", false)]
    [TestCase("2", false)]
    public async Task Validate_Patch_HasOnlineDeliveryOption_VariousFieldValues_MatchingErrors(string hasOnlineDeliveryOptionValue, bool isValid)
    {
        var validator = new PatchProviderCourseCommandValidator(_providersReadRepo.Object, _providerCoursesReadRepo.Object);
        var ukprn = 10000001;
        var larsCode = "1";

        var command = new PatchProviderCourseCommand
        {
            Ukprn = ukprn,
            LarsCode = larsCode,
            Patch = new JsonPatchDocument<Domain.Models.PatchProviderCourse>()
        };

        command.Patch = new JsonPatchDocument<Domain.Models.PatchProviderCourse>
        {
            Operations =
            {
                new Operation<Domain.Models.PatchProviderCourse>
                    { op = Replace, path = HasOnlineDeliveryOption, value = hasOnlineDeliveryOptionValue }
            }
        };

        var result = await validator.TestValidateAsync(command);
        if (isValid)
        {
            result.ShouldNotHaveAnyValidationErrors();
            result.IsValid.Should().BeTrue();
        }
        else
        {
            result.IsValid.Should().BeFalse();
            result.Errors.Should().HaveCount(1);
            PatchProviderCourseCommandValidator.HasOnlineDeliveryOptionIsNotABooleanErrorMessage.Should().Be(result.Errors[0].ErrorMessage);
        }
    }
}