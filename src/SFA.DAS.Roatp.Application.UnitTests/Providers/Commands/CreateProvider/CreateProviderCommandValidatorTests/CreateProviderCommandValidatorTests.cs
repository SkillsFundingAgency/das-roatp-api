using System.Threading.Tasks;
using FluentAssertions;
using FluentValidation.TestHelper;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Application.Providers.Commands.CreateProvider;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.UnitTests.Providers.Commands.CreateProvider.CreateProviderCommandValidatorTests;

[TestFixture]
public class CreateProviderCommandValidatorTests
{
    private Mock<IProvidersReadRepository> _providersReadRepositoryMock;

    public const int UkprnAlreadyPresent = 10012002;
    public const int UkprnNotAlreadyPresent = 11111111;

    [TestCase(1111111, false, UkprnValidator.InvalidUkprnErrorMessage)]
    [TestCase(111111111, false, UkprnValidator.InvalidUkprnErrorMessage)]
    [TestCase(UkprnNotAlreadyPresent, true, null)]
    [TestCase(UkprnAlreadyPresent, false, CreateProviderCommandValidator.UkprnAlreadyPresent)]
    public async Task Ukprn_Validation(int ukprn, bool isValid, string errorMessage)
    {
        var command = new CreateProviderCommand { Ukprn = ukprn, LegalName = "legal name", UserDisplayName = "display name", UserId = "user id" };
        var sut = GetValidator();

        var result = await sut.TestValidateAsync(command);

        if (isValid)
            result.ShouldNotHaveValidationErrorFor(c => c.Ukprn);
        else
        {
            result.ShouldHaveValidationErrorFor(c => c.Ukprn);
            result.Errors[0].ErrorMessage.Should().Be(errorMessage);
        }
    }

    [TestCase("legal name", true)]
    [TestCase("", false)]
    [TestCase(null, false)]
    public async Task LegalNamePresent_Validation(string legalName, bool isValid)
    {
        var command = new CreateProviderCommand { Ukprn = UkprnNotAlreadyPresent, LegalName = legalName, UserDisplayName = "display name", UserId = "user id" };
        var sut = GetValidator();

        var result = await sut.TestValidateAsync(command);
        if (isValid)
            result.ShouldNotHaveValidationErrorFor(c => c.LegalName);
        else
        {
            result.ShouldHaveValidationErrorFor(c => c.LegalName);
            result.Errors[0].ErrorMessage.Should().Be(CreateProviderCommandValidator.LegalNameRequired);
        }
    }

    [TestCase(TestHelper.Constants.AllowedSpecialCharacters, true)]
    [TestCase("<", false)]
    [TestCase(":", false)]
    [TestCase("=", false)]
    public async Task ValidateNames_ForSpecialCharacters(string value, bool isValid)
    {
        var command = new CreateProviderCommand { Ukprn = UkprnNotAlreadyPresent, LegalName = value, TradingName = value, UserDisplayName = value, UserId = value };
        var sut = GetValidator();
        var result = await sut.TestValidateAsync(command);
        if (isValid)
        {
            result.ShouldNotHaveValidationErrorFor(c => c.LegalName);
            result.ShouldNotHaveValidationErrorFor(c => c.TradingName);
            result.ShouldNotHaveValidationErrorFor(c => c.UserDisplayName);
            result.ShouldNotHaveValidationErrorFor(c => c.UserId);
        }
        else
        {
            result.ShouldHaveValidationErrorFor(c => c.LegalName).WithErrorMessage(ValidationMessages.InvalidCharactersErrorMessage);
            result.ShouldHaveValidationErrorFor(c => c.TradingName).WithErrorMessage(ValidationMessages.InvalidCharactersErrorMessage);
            result.ShouldHaveValidationErrorFor(c => c.UserDisplayName).WithErrorMessage(ValidationMessages.InvalidCharactersErrorMessage);
            result.ShouldHaveValidationErrorFor(c => c.UserId).WithErrorMessage(ValidationMessages.InvalidCharactersErrorMessage);
        }
    }

    protected CreateProviderCommandValidator GetValidator()
    {
        _providersReadRepositoryMock = new Mock<IProvidersReadRepository>();
        _providersReadRepositoryMock.Setup(p => p.GetByUkprn(UkprnAlreadyPresent)).ReturnsAsync(new Provider { Ukprn = UkprnAlreadyPresent });

        return new CreateProviderCommandValidator(_providersReadRepositoryMock.Object);
    }
}
