using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using FluentValidation.TestHelper;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.Locations.Commands.DeleteLocation;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Testing.AutoFixture;

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


    [Test, RecursiveMoqAutoData]
    public async Task ValidateCoursePresentAtOtherLocations_ReturnsNoErrors(
        [Frozen] Mock<IProvidersReadRepository> providerReadRepository,
        [Frozen] Mock<IProviderLocationsReadRepository> providerLocationsReadRepository,
        [Greedy] DeleteProviderLocationCommandValidator sut,
        ProviderLocation providerLocation)
    {
        var command = new DeleteProviderLocationCommand(_ukprn, _navigationId, _userId, _userDisplayName);
        providerLocationsReadRepository.Setup(x => x.GetProviderLocation(_ukprn, _navigationId))
            .ReturnsAsync(providerLocation);
        providerLocationsReadRepository.Setup(x => x.GetProviderCoursesByLocation(_ukprn, _navigationId)).ReturnsAsync(new List<Domain.Entities.ProviderCourse> { new() });


        var result = await sut.TestValidateAsync(command);

        result.ShouldNotHaveValidationErrorFor(c => c.UserDisplayName);
    }

    [Test, RecursiveMoqAutoData]
    public async Task ValidateCoursePresentAtOtherLocations_Validation_Fails(
        [Frozen] Mock<IProvidersReadRepository> providerReadRepository,
        [Frozen] Mock<IProviderLocationsReadRepository> providerLocationsReadRepository,
        [Greedy] DeleteProviderLocationCommandValidator sut,
        ProviderLocation providerLocation)
    {
        var command = new DeleteProviderLocationCommand(_ukprn, _navigationId, _userId, _userDisplayName);
        providerLocationsReadRepository.Setup(x => x.GetProviderLocation(_ukprn, _navigationId))
            .ReturnsAsync(providerLocation);
        providerLocationsReadRepository.Setup(x => x.GetProviderCoursesByLocation(_ukprn, _navigationId)).ReturnsAsync(new List<Domain.Entities.ProviderCourse> { new() });

        var result = await sut.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(c => c.Id);
        result.Errors.Count.Should().Be(1);
        result.Errors[0].ErrorMessage.Should()
            .Be(DeleteProviderLocationCommandValidator.ProviderLocationOrphanedStandardErrorMessage);
    }

    private static DeleteProviderLocationCommandValidator GetDefaultValidator()
    {
        var providerLocationsReadRepositoryMock = new Mock<IProviderLocationsReadRepository>();

        providerLocationsReadRepositoryMock
            .Setup(x => x.GetProviderCoursesByLocation(It.IsAny<int>(), It.IsAny<Guid>()))
            .ReturnsAsync(new List<Domain.Entities.ProviderCourse> { new() });
        return new DeleteProviderLocationCommandValidator(Mock.Of<IProvidersReadRepository>(),
            providerLocationsReadRepositoryMock.Object);
    }
}

