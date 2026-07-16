using System;
using System.Threading.Tasks;
using AutoFixture.NUnit4;
using FluentValidation.TestHelper;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.ProviderAllowedCourses.Commands.UpsertProviderAllowedCourse;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.Application.UnitTests.ProviderAllowedCourses.Commands.UpsertProviderAllowedCourse;

public class UpsertProviderAllowedCourseCommandValidatorTests
{
    [Test, MoqAutoData]
    public async Task WhenLastDateStartsIsAfterStandardDate_ThenValidationShouldFail(
        [Frozen] Mock<IStandardsReadRepository> standardsReadRepository,
        [Frozen] Mock<IProvidersReadRepository> providersReadRepository,
        [Greedy] UpsertProviderAllowedCourseCommandValidator sut)
    {
        // Arrange
        var command = new UpsertProviderAllowedCourseCommand
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
            .WithErrorMessage(UpsertProviderAllowedCourseCommandValidator.InvalidLastDateStarts);
    }

    [Test, MoqAutoData]
    public async Task WhenLastDateStartsIsBeforeStandardDate_ThenValidationShouldPass(
        [Frozen] Mock<IStandardsReadRepository> standardsReadRepository,
        [Frozen] Mock<IProvidersReadRepository> providersReadRepository,
        [Greedy] UpsertProviderAllowedCourseCommandValidator sut)
    {
        // Arrange
        var command = new UpsertProviderAllowedCourseCommand
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
    public async Task WhenLastDateStartsIsNull_ThenValidationShouldPass(
        [Frozen] Mock<IStandardsReadRepository> standardsReadRepository,
        [Frozen] Mock<IProvidersReadRepository> providersReadRepository,
        [Greedy] UpsertProviderAllowedCourseCommandValidator sut)
    {
        // Arrange
        var command = new UpsertProviderAllowedCourseCommand
        {
            Ukprn = 12345678,
            LarsCode = "12345",
            UserId = "TestUserId",
            UserDisplayName = "TestUser",
            LastDateStarts = null
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
}
