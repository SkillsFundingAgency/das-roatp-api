using System.Collections.Generic;
using System.Threading.Tasks;
using FluentValidation.TestHelper;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Application.ProviderContact.Commands.CreateProviderContact;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.Application.UnitTests.ProviderContact.PostProviderContact;

[TestFixture]
public class PostProviderContactValidatorTests
{
    const string userDisplayName = "test";
    const string userId = "test";
    private const int ukprn = 10012002;

    [Test, MoqAutoData]
    public async Task ValidateUkprn_InValid_ReturnsError()
    {
        var ukprn = 10000000;
        var command = new CreateProviderContactCommand();
        command.Ukprn = ukprn;
        var sut = new CreateProviderContactCommandValidator(Mock.Of<IProvidersReadRepository>(), Mock.Of<IProviderCoursesReadRepository>());

        var result = await sut.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(c => c.Ukprn).WithErrorMessage(UkprnValidator.InvalidUkprnErrorMessage);
    }

    [TestCase("")]
    [TestCase(null)]
    [TestCase(" ")]
    public async Task ValidateUserId_Empty_ReturnsError(string userId)
    {
        var command = new CreateProviderContactCommand { Ukprn = 10012002, UserId = userId, UserDisplayName = userDisplayName };
        var sut = new CreateProviderContactCommandValidator(Mock.Of<IProvidersReadRepository>(), Mock.Of<IProviderCoursesReadRepository>());
        var result = await sut.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(c => c.UserId);
    }

    [TestCase("")]
    [TestCase(null)]
    [TestCase(" ")]
    public async Task ValidateUserDisplayName_Empty_ReturnsError(string userDisplayName)
    {
        var command = new CreateProviderContactCommand { Ukprn = 10012002, UserId = userId, UserDisplayName = userDisplayName };
        var sut = new CreateProviderContactCommandValidator(Mock.Of<IProvidersReadRepository>(), Mock.Of<IProviderCoursesReadRepository>());
        var result = await sut.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(c => c.UserDisplayName);
    }

    [Test]
    public async Task ValidateEmail_TooLong_ReturnsError()
    {
        var command = new CreateProviderContactCommand { Ukprn = ukprn, UserId = userId, UserDisplayName = userDisplayName };
        string emailTooLong = new string('x', 300);
        command.EmailAddress = $"test@{emailTooLong}.com";
        var sut = new CreateProviderContactCommandValidator(Mock.Of<IProvidersReadRepository>(), Mock.Of<IProviderCoursesReadRepository>());
        var result = await sut.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(c => c.EmailAddress).WithErrorMessage(CreateProviderContactCommandValidator.EmailAddressTooLong);
    }

    [Test]
    public async Task ValidateEmail_WrongFormat_ReturnsError()
    {
        var command = new CreateProviderContactCommand { Ukprn = ukprn, UserId = userId, UserDisplayName = userDisplayName };
        command.EmailAddress = $"aaa";
        var sut = new CreateProviderContactCommandValidator(Mock.Of<IProvidersReadRepository>(), Mock.Of<IProviderCoursesReadRepository>());
        var result = await sut.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(c => c.EmailAddress).WithErrorMessage(CreateProviderContactCommandValidator.EmailAddressWrongFormat);
    }

    [Test]
    public async Task ValidatePhoneNumber_TooLong_ReturnsError()
    {
        var command = new CreateProviderContactCommand { Ukprn = ukprn, UserId = userId, UserDisplayName = userDisplayName };
        string phoneNumberTooLong = new string('x', 51);
        command.PhoneNumber = phoneNumberTooLong;
        var sut = new CreateProviderContactCommandValidator(Mock.Of<IProvidersReadRepository>(), Mock.Of<IProviderCoursesReadRepository>());
        var result = await sut.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(c => c.PhoneNumber).WithErrorMessage(CreateProviderContactCommandValidator.PhoneNumberTooLong);
    }

    [Test]
    public async Task ValidateMissingEmailAndPhoneNumber_ReturnsError()
    {
        var command = new CreateProviderContactCommand { Ukprn = ukprn, UserId = userId, UserDisplayName = userDisplayName };
        var sut = new CreateProviderContactCommandValidator(Mock.Of<IProvidersReadRepository>(), Mock.Of<IProviderCoursesReadRepository>());
        var result = await sut.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(c => c.EmailAddress).WithErrorMessage(CreateProviderContactCommandValidator.EmailOrPhoneNumberNeedsValue);
    }

    [Test]
    public async Task Validate_ProviderCourseIdNotMatched_ReturnsError()
    {
        var providerCourseIdsToCheck = new List<int> { 1, 2 };
        var providerCourseIdsToMatch = new List<Domain.Entities.ProviderCourse>
        {
            new() {Id = 2},
            new() { Id = 3}
        };

        var command = new CreateProviderContactCommand { Ukprn = ukprn, UserId = userId, UserDisplayName = userDisplayName, ProviderCourseIds = providerCourseIdsToCheck, EmailAddress = "test@test.com" };
        var providerCoursesReadRepositoryMock = new Mock<IProviderCoursesReadRepository>();
        providerCoursesReadRepositoryMock.Setup(x => x.GetAllProviderCourses(ukprn)).ReturnsAsync(providerCourseIdsToMatch);

        var sut = new CreateProviderContactCommandValidator(Mock.Of<IProvidersReadRepository>(), providerCoursesReadRepositoryMock.Object);
        var result = await sut.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(c => c.ProviderCourseIds).WithErrorMessage(CreateProviderContactCommandValidator.ProviderCourseIdDoesNotExist);
    }
}
