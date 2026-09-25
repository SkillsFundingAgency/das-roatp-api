using System.Threading.Tasks;
using FluentValidation.TestHelper;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.Common;

namespace SFA.DAS.Roatp.Application.UnitTests.Common;

[TestFixture]
public class UserInfoValidatorTests
{
    UserInfoValidator _sut;
    const string ValidUserId = "TestUserId";
    const string ValidUserDisplayName = "TestUserDisplayName";

    [TestCase("", UserInfoValidator.UserIdEmptyErrorMessage, false)]
    [TestCase("  ", UserInfoValidator.UserIdEmptyErrorMessage, false)]
    [TestCase(null, UserInfoValidator.UserIdEmptyErrorMessage, false)]
    [TestCase("test", "", true)]
    public async Task UserInfoValidator_UserId_Validations(string userId, string expectedErrorMessage, bool isValid)
    {
        _sut = new UserInfoValidator();
        var testObj = new UserInfoValidatorTestObject(userId, ValidUserDisplayName);

        var result = await _sut.TestValidateAsync(testObj);

        if (isValid)
        {
            result.ShouldNotHaveValidationErrorFor(c => c.UserId);
        }
        else
        {
            result.ShouldHaveValidationErrorFor(c => c.UserId).WithErrorMessage(expectedErrorMessage);
        }
    }

    [TestCase("(_.#*$&%~'?`+_@ -", true)]
    [TestCase("<", false)]
    [TestCase(":", false)]
    [TestCase("=", false)]
    [TestCase("{", false)]
    public void UserInfoValidator_CharacterValidations(string value, bool isValid)
    {
        _sut = new UserInfoValidator();
        var testObj = new UserInfoValidatorTestObject(value, value);
        var result = _sut.TestValidate(testObj);

        if (isValid)
        {
            result.ShouldNotHaveValidationErrorFor(c => c.UserId);
            result.ShouldNotHaveValidationErrorFor(c => c.UserDisplayName);
        }
        else
        {
            result.ShouldHaveValidationErrorFor(c => c.UserId).WithErrorMessage(ValidationMessages.InvalidCharactersErrorMessage);
            result.ShouldHaveValidationErrorFor(c => c.UserDisplayName).WithErrorMessage(ValidationMessages.InvalidCharactersErrorMessage);
        }
    }

    [TestCase(256, true)]
    [TestCase(257, false)]
    public void UserInfoValidator_UserId_CharacterCountValidations(int characterCount, bool isValid)
    {
        _sut = new UserInfoValidator();
        var value = new string('a', characterCount);
        var testObj = new UserInfoValidatorTestObject(value, value);
        var result = _sut.TestValidate(testObj);

        if (isValid)
        {
            result.ShouldNotHaveValidationErrorFor(c => c.UserId);
            result.ShouldNotHaveValidationErrorFor(c => c.UserDisplayName);
        }
        else
        {
            result.ShouldHaveValidationErrorFor(c => c.UserId).WithErrorMessage(UserInfoValidator.MaximumAllowedLengthErrorMessage);
            result.ShouldHaveValidationErrorFor(c => c.UserDisplayName).WithErrorMessage(UserInfoValidator.MaximumAllowedLengthErrorMessage);
        }
    }

    [TestCase("", UserInfoValidator.UserDisplayNameEmptyErrorMessage, false)]
    [TestCase("  ", UserInfoValidator.UserDisplayNameEmptyErrorMessage, false)]
    [TestCase(null, UserInfoValidator.UserDisplayNameEmptyErrorMessage, false)]
    [TestCase("test", "", true)]
    public async Task UserInfoValidator_UserDisplayName_Validations(string userDisplayName, string expectedErrorMessage, bool isValid)
    {
        _sut = new UserInfoValidator();
        var testObj = new UserInfoValidatorTestObject(ValidUserId, userDisplayName);

        var result = await _sut.TestValidateAsync(testObj);

        if (isValid)
            result.ShouldNotHaveValidationErrorFor(c => c.UserDisplayName);
        else
            result.ShouldHaveValidationErrorFor(c => c.UserDisplayName).WithErrorMessage(expectedErrorMessage);
    }
}

public class UserInfoValidatorTestObject : IUserInfo
{
    public string UserId { get; }

    public string UserDisplayName { get; }
    public UserInfoValidatorTestObject(string userId, string userDisplayName)
    {
        UserId = userId;
        UserDisplayName = userDisplayName;
    }
}
