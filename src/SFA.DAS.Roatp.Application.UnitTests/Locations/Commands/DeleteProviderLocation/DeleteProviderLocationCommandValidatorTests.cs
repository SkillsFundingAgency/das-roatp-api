using System;
using System.Threading.Tasks;
using FluentValidation.TestHelper;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.Locations.Commands.DeleteLocation;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.UnitTests.Locations.Commands.DeleteProviderLocation;

[TestFixture]
public class DeleteProviderLocationCommandValidatorTests
{
    private readonly string _userId = "userid";
    private readonly string _userDisplayName = "userDisplayName";
    private readonly Guid _navigationId = Guid.NewGuid();
    private readonly int _ukprn = 10000001;

    [Test]
    public async Task Validates_Ukprn_ReturnsError()
    {
        var command = new DeleteProviderLocationCommand(_ukprn, _navigationId, _userId, _userDisplayName);
        var sut = GetDefaultValidator();
        var result = await sut.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(c => c.Ukprn);
    }

    [Test]
    public async Task ValidateNavigationId_Empty_ReturnsError()
    {
        var command = new DeleteProviderLocationCommand(_ukprn, _navigationId, _userId, _userDisplayName);
        var sut = GetDefaultValidator();
        var result = await sut.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(c => c.Id);
    }

    [TestCase("")]
    [TestCase(null)]
    [TestCase(" ")]
    public async Task ValidateUserId_Empty_ReturnsError(string userId)
    {
        var command = new DeleteProviderLocationCommand(_ukprn, _navigationId, userId, _userDisplayName);
        var sut = GetDefaultValidator();
        var result = await sut.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(c => c.UserId);
    }

    [TestCase("Test")]
    public async Task ValidateUserId_NotEmpty_ReturnsNoErrors(string userId)
    {
        var command = new DeleteProviderLocationCommand(_ukprn, _navigationId, userId, _userDisplayName);
        var sut = GetDefaultValidator();
        var result = await sut.TestValidateAsync(command);

        result.ShouldNotHaveValidationErrorFor(c => c.UserId);
    }

    [TestCase("")]
    [TestCase(null)]
    [TestCase(" ")]
    public async Task ValidateUserDisplayName_Empty_ReturnsError(string userDisplayName)
    {
        var command = new DeleteProviderLocationCommand(_ukprn, _navigationId, _userId, userDisplayName);
        var sut = GetDefaultValidator();
        var result = await sut.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(c => c.UserDisplayName);
    }

    [TestCase("Test")]
    public async Task ValidateUserDisplayName_NotEmpty_ReturnsNoErrors(string userDisplayName)
    {
        var command = new DeleteProviderLocationCommand(_ukprn, _navigationId, _userId, userDisplayName);
        var sut = GetDefaultValidator();
        var result = await sut.TestValidateAsync(command);

        result.ShouldNotHaveValidationErrorFor(c => c.UserDisplayName);
    }

    private DeleteProviderLocationCommandValidator GetDefaultValidator() => new DeleteProviderLocationCommandValidator(Mock.Of<IProvidersReadRepository>(), Mock.Of<IProviderLocationsReadRepository>());
}

