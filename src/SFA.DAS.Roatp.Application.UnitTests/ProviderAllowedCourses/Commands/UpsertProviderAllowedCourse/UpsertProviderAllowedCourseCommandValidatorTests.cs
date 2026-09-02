using System;
using System.Collections.Generic;
using System.Threading;
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
        [Greedy] UpsertProviderAllowedCourseCommandValidator sut)
    {
        // Arrange
        var command = new UpsertProviderAllowedCourseCommand
        {
            Ukprn = 12345678,
            LarsCode = "12345",
            UserId = "TestUserId",
            UserDisplayName = "TestUser",
            LastDateStarts = DateTime.UtcNow,
            IsStartRestricted = false
        };

        standardsReadRepository
            .Setup(r => r.GetStandard(It.IsAny<string>()))
            .ReturnsAsync(new Standard { LarsCode = command.LarsCode, LastDateStarts = DateTime.UtcNow.AddMonths(-1) });

        // Act
        var result = await sut.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x)
            .WithErrorMessage(UpsertProviderAllowedCourseCommandValidator.InvalidLastDateStarts);
    }

    [Test, MoqAutoData]
    public async Task WhenLastDateStartsIsBeforeStandardDate_ThenValidationShouldPass(
        [Frozen] Mock<IStandardsReadRepository> standardsReadRepository,
        [Greedy] UpsertProviderAllowedCourseCommandValidator sut)
    {
        // Arrange
        var command = new UpsertProviderAllowedCourseCommand
        {
            Ukprn = 12345678,
            LarsCode = "12345",
            UserId = "TestUserId",
            UserDisplayName = "TestUser",
            LastDateStarts = DateTime.UtcNow.AddMonths(-1),
            IsStartRestricted = false
        };

        standardsReadRepository
            .Setup(r => r.GetStandard(It.IsAny<string>()))
            .ReturnsAsync(new Standard { LarsCode = command.LarsCode, LastDateStarts = DateTime.UtcNow });

        // Act
        var result = await sut.TestValidateAsync(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x);
    }

    [Test, MoqAutoData]
    public async Task WhenLastDateStartsIsNull_ThenValidationShouldPass_AndVerifyNoStandardsReadRepositoryCall(
        [Frozen] Mock<IStandardsReadRepository> standardsReadRepository,
        [Greedy] UpsertProviderAllowedCourseCommandValidator sut)
    {
        // Arrange
        var command = new UpsertProviderAllowedCourseCommand
        {
            Ukprn = 12345678,
            LarsCode = "12345",
            UserId = "TestUserId",
            UserDisplayName = "TestUser",
            LastDateStarts = null,
            IsStartRestricted = false
        };

        standardsReadRepository
            .Setup(r => r.GetStandard(It.IsAny<string>()))
            .ReturnsAsync(new Standard { LarsCode = command.LarsCode, LastDateStarts = DateTime.UtcNow });

        // Act
        var result = await sut.TestValidateAsync(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x);
        standardsReadRepository.Verify(r => r.GetStandard(It.Is<string>(x => x == command.LarsCode)), Times.Never);
    }

    [Test, MoqAutoData]
    public async Task WhenIsStartRestrictedIsTrue_AndExistsInProviderAllowedCourse_ThenValidationShouldFail(
        [Frozen] Mock<IStandardsReadRepository> standardsReadRepository,
        [Frozen] Mock<IProviderAllowedCoursesRepository> providerAllowedCoursesRepository,
        [Greedy] UpsertProviderAllowedCourseCommandValidator sut)
    {
        // Arrange
        var command = new UpsertProviderAllowedCourseCommand
        {
            Ukprn = 12345678,
            LarsCode = "12345",
            UserId = "TestUserId",
            UserDisplayName = "TestUser",
            LastDateStarts = DateTime.UtcNow.AddMonths(-1),
            IsStartRestricted = true
        };

        var providerAllowedCourse = new List<ProviderAllowedCourse>()
        {
            new()
            {
                LarsCode = command.LarsCode,
                Ukprn = command.Ukprn,
            }
        };

        standardsReadRepository
            .Setup(r => r.GetStandard(It.IsAny<string>()))
            .ReturnsAsync(new Standard { LarsCode = command.LarsCode, LastDateStarts = DateTime.UtcNow });

        providerAllowedCoursesRepository
            .Setup(r => r.GetProviderAllowedCoursesByLarsCode(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(providerAllowedCourse);

        // Act
        var result = await sut.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x)
            .WithErrorMessage(UpsertProviderAllowedCourseCommandValidator.ExistsInProviderAllowedCourse);
    }

    [Test, MoqAutoData]
    public async Task WhenIsStartRestrictedIsTrue_AndDoesNotExistInProviderAllowedCourse_ThenValidationShouldPass(
        [Frozen] Mock<IStandardsReadRepository> standardsReadRepository,
        [Frozen] Mock<IProviderAllowedCoursesRepository> providerAllowedCoursesRepository,
        [Greedy] UpsertProviderAllowedCourseCommandValidator sut)
    {
        // Arrange
        var command = new UpsertProviderAllowedCourseCommand
        {
            Ukprn = 12345678,
            LarsCode = "12345",
            UserId = "TestUserId",
            UserDisplayName = "TestUser",
            LastDateStarts = DateTime.UtcNow.AddMonths(-1),
            IsStartRestricted = true
        };

        var providerAllowedCourse = new List<ProviderAllowedCourse>();

        standardsReadRepository
            .Setup(r => r.GetStandard(It.IsAny<string>()))
            .ReturnsAsync(new Standard { LarsCode = command.LarsCode, LastDateStarts = DateTime.UtcNow });

        providerAllowedCoursesRepository
            .Setup(r => r.GetProviderAllowedCoursesByLarsCode(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(providerAllowedCourse);

        // Act
        var result = await sut.TestValidateAsync(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x);
    }

    [Test, MoqAutoData]
    public async Task WhenIsStartRestrictedIsFalse_ThenValidationShouldPass_AndVerifyProviderAllowedCoursesRepositoryIsNotCalled(
        [Frozen] Mock<IStandardsReadRepository> standardsReadRepository,
        [Frozen] Mock<IProviderAllowedCoursesRepository> providerAllowedCoursesRepository,
        [Greedy] UpsertProviderAllowedCourseCommandValidator sut)
    {
        // Arrange
        var command = new UpsertProviderAllowedCourseCommand
        {
            Ukprn = 12345678,
            LarsCode = "12345",
            UserId = "TestUserId",
            UserDisplayName = "TestUser",
            LastDateStarts = DateTime.UtcNow.AddMonths(-1),
            IsStartRestricted = false
        };

        var providerAllowedCourse = new List<ProviderAllowedCourse>()
        {
            new()
            {
                LarsCode = command.LarsCode,
                Ukprn = command.Ukprn,
            }
        };

        standardsReadRepository
            .Setup(r => r.GetStandard(It.IsAny<string>()))
            .ReturnsAsync(new Standard { LarsCode = command.LarsCode, LastDateStarts = DateTime.UtcNow });

        providerAllowedCoursesRepository
            .Setup(r => r.GetProviderAllowedCoursesByLarsCode(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(providerAllowedCourse);

        // Act
        var result = await sut.TestValidateAsync(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x);
        providerAllowedCoursesRepository
            .Verify(r => r.GetProviderAllowedCoursesByLarsCode(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
