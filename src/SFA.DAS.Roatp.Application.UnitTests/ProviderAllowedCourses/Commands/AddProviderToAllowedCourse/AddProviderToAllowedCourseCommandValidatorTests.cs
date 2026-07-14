using System;
using System.Threading.Tasks;
using AutoFixture.NUnit4;
using FluentValidation.TestHelper;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Application.ProviderAllowedCourses.Commands.AddProviderToAllowedCourse;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.Application.UnitTests.ProviderAllowedCourses.Commands.AddProviderToAllowedCourse;

public class AddProviderToAllowedCourseCommandValidatorTests
{
    [Test, MoqAutoData]
    public async Task WhenLastDateStartsIsAfterStandardDate_ThenValidationShouldFail(
        [Frozen] Mock<IStandardsReadRepository> standardsReadRepository,
        [Frozen] Mock<IProvidersReadRepository> providersReadRepository,
        [Greedy] AddProviderToAllowedCourseCommandValidator sut)
    {
        // Arrange
        var command = new AddProviderToAllowedCourseCommand
        {
            Ukprn = 12345678,
            LarsCode = "12345",
            UserId = "TestUserId",
            UserDisplayName = "TestUser",
            LastDateStarts = DateTime.UtcNow
        };

        standardsReadRepository
            .Setup(r => r.GetStandard(It.IsAny<string>()))
            .ReturnsAsync(new Standard { LarsCode = command.LarsCode, LastDateStarts = DateTime.UtcNow.AddMonths(-1) });

        providersReadRepository
            .Setup(r => r.GetByUkprn(It.IsAny<int>()))
            .ReturnsAsync((Provider)null);

        // Act
        var result = await sut.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x)
            .WithErrorMessage(AddProviderToAllowedCourseCommandValidator.InvalidLastDateStarts);
    }

    [Test, MoqAutoData]
    public async Task WhenLastDateStartsIsBeforeStandardDate_ThenValidationShouldPass(
        [Frozen] Mock<IStandardsReadRepository> standardsReadRepository,
        [Frozen] Mock<IProvidersReadRepository> providersReadRepository,
        [Greedy] AddProviderToAllowedCourseCommandValidator sut)
    {
        // Arrange
        var command = new AddProviderToAllowedCourseCommand
        {
            Ukprn = 12345678,
            LarsCode = "12345",
            UserId = "TestUserId",
            UserDisplayName = "TestUser",
            LastDateStarts = DateTime.UtcNow.AddMonths(-1)
        };

        standardsReadRepository
            .Setup(r => r.GetStandard(It.IsAny<string>()))
            .ReturnsAsync(new Standard { LarsCode = command.LarsCode, LastDateStarts = DateTime.UtcNow });

        providersReadRepository
            .Setup(r => r.GetByUkprn(It.IsAny<int>()))
            .ReturnsAsync((Provider)null);

        // Act
        var result = await sut.TestValidateAsync(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x);
    }

    [Test, MoqAutoData]
    public async Task WhenLarsCodeDoesNotExist_ThenValidationShouldFail(
    [Frozen] Mock<IStandardsReadRepository> standardsReadRepository,
    [Frozen] Mock<IProvidersReadRepository> providersReadRepository,
    [Greedy] AddProviderToAllowedCourseCommandValidator sut)
    {
        // Arrange
        var command = new AddProviderToAllowedCourseCommand
        {
            Ukprn = 12345678,
            LarsCode = "12345",
            UserId = "TestUserId",
            UserDisplayName = "TestUser",
            LastDateStarts = DateTime.UtcNow
        };

        standardsReadRepository
            .Setup(r => r.GetStandard(It.IsAny<string>()))
            .ReturnsAsync((Standard)null);

        providersReadRepository
            .Setup(r => r.GetByUkprn(It.IsAny<int>()))
            .ReturnsAsync((Provider)null);

        // Act
        var result = await sut.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.LarsCode)
            .WithErrorMessage(LarsCodeValidator.NotFoundMessage);
    }

    [Test, MoqAutoData]
    public async Task WhenLarsCodeExists_ThenValidationShouldPass(
        [Frozen] Mock<IStandardsReadRepository> standardsReadRepository,
        [Frozen] Mock<IProvidersReadRepository> providersReadRepository,
        [Greedy] AddProviderToAllowedCourseCommandValidator sut)
    {
        // Arrange
        var command = new AddProviderToAllowedCourseCommand
        {
            Ukprn = 12345678,
            LarsCode = "12345",
            UserId = "TestUserId",
            UserDisplayName = "TestUser",
            LastDateStarts = DateTime.UtcNow
        };

        standardsReadRepository
            .Setup(r => r.GetStandard(It.IsAny<string>()))
            .ReturnsAsync(new Standard { LarsCode = command.LarsCode });

        providersReadRepository
            .Setup(r => r.GetByUkprn(It.IsAny<int>()))
            .ReturnsAsync((Provider)null);

        // Act
        var result = await sut.TestValidateAsync(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.LarsCode);
    }

    [Test, MoqAutoData]
    public async Task WhenUkprnDoesNotExist_ThenValidationShouldFail(
    [Frozen] Mock<IStandardsReadRepository> standardsReadRepository,
    [Frozen] Mock<IProvidersReadRepository> providersReadRepository,
    [Greedy] AddProviderToAllowedCourseCommandValidator sut)
    {
        // Arrange
        var command = new AddProviderToAllowedCourseCommand
        {
            Ukprn = 12345678,
            LarsCode = "12345",
            UserId = "TestUserId",
            UserDisplayName = "TestUser",
            LastDateStarts = DateTime.UtcNow
        };

        standardsReadRepository
            .Setup(r => r.GetStandard(It.IsAny<string>()))
            .ReturnsAsync(new Standard { LarsCode = command.LarsCode });

        providersReadRepository
            .Setup(r => r.GetByUkprn(It.IsAny<int>()))
            .ReturnsAsync((Provider)null);

        // Act
        var result = await sut.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Ukprn)
            .WithErrorMessage(UkprnValidator.ProviderNotFoundErrorMessage);
    }

    [Test, MoqAutoData]
    public async Task WhenUkprnExists_ThenValidationShouldPass(
        [Frozen] Mock<IStandardsReadRepository> standardsReadRepository,
        [Frozen] Mock<IProvidersReadRepository> providersReadRepository,
        [Greedy] AddProviderToAllowedCourseCommandValidator sut)
    {
        // Arrange
        var command = new AddProviderToAllowedCourseCommand
        {
            Ukprn = 12345678,
            LarsCode = "12345",
            UserId = "TestUserId",
            UserDisplayName = "TestUser",
            LastDateStarts = DateTime.UtcNow
        };

        standardsReadRepository
            .Setup(r => r.GetStandard(It.IsAny<string>()))
            .ReturnsAsync(new Standard { LarsCode = command.LarsCode });

        providersReadRepository
            .Setup(r => r.GetByUkprn(It.IsAny<int>()))
            .ReturnsAsync(new Provider { Ukprn = command.Ukprn });

        // Act
        var result = await sut.TestValidateAsync(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Ukprn);
    }
}
