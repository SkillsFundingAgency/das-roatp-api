using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit4;
using FluentValidation.TestHelper;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.ProviderAllowedCourses.Commands.CreateProviderAllowedCourse;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.Application.UnitTests.ProviderAllowedCourses.Commands.CreateProviderAllowedCourse;

public class CreateProviderAllowedCourseCommandValidatorTests
{
    [Test, MoqAutoData]
    public async Task WhenLastDateStartsIsAfterStandardDate_ThenValidationShouldFail(
        [Frozen] Mock<IStandardsReadRepository> standardsReadRepository,
        [Frozen] Mock<IProviderAllowedCoursesRepository> providerAllowedCoursesRepository,
        [Greedy] CreateProviderAllowedCourseCommandValidator sut)
    {
        // Arrange
        var command = new CreateProviderAllowedCourseCommand
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

        providerAllowedCoursesRepository
            .Setup(r => r.GetProviderAllowedCoursesByLarsCode(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ProviderAllowedCourse>());

        // Act
        var result = await sut.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x)
            .WithErrorMessage(CreateProviderAllowedCourseCommandValidator.InvalidLastDateStarts);
    }

    [Test, MoqAutoData]
    public async Task WhenLastDateStartsIsBeforeStandardDate_ThenValidationShouldPass(
        [Frozen] Mock<IStandardsReadRepository> standardsReadRepository,
        [Frozen] Mock<IProviderAllowedCoursesRepository> providerAllowedCoursesRepository,
        [Greedy] CreateProviderAllowedCourseCommandValidator sut)
    {
        // Arrange
        var command = new CreateProviderAllowedCourseCommand
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

        providerAllowedCoursesRepository
            .Setup(r => r.GetProviderAllowedCoursesByLarsCode(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ProviderAllowedCourse>());

        // Act
        var result = await sut.TestValidateAsync(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x);
    }

    [Test, MoqAutoData]
    public async Task WhenLastDateStartsIsNull_ThenValidationShouldPass_AndVerifyNoStandardsReadRepositoryCall(
        [Frozen] Mock<IStandardsReadRepository> standardsReadRepository,
        [Frozen] Mock<IProviderAllowedCoursesRepository> providerAllowedCoursesRepository,
        [Greedy] CreateProviderAllowedCourseCommandValidator sut)
    {
        // Arrange
        var command = new CreateProviderAllowedCourseCommand
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

        providerAllowedCoursesRepository
            .Setup(r => r.GetProviderAllowedCoursesByLarsCode(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ProviderAllowedCourse>());

        // Act
        var result = await sut.TestValidateAsync(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x);
        standardsReadRepository.Verify(r => r.GetStandard(It.Is<string>(x => x == command.LarsCode)), Times.Never);
    }

    [Test, MoqAutoData]
    public async Task WhenExistsInProviderAllowedCourse_ThenValidationShouldFail(
        [Frozen] Mock<IStandardsReadRepository> standardsReadRepository,
        [Frozen] Mock<IProviderAllowedCoursesRepository> providerAllowedCoursesRepository,
        [Greedy] CreateProviderAllowedCourseCommandValidator sut)
    {
        // Arrange
        var command = new CreateProviderAllowedCourseCommand
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
            .WithErrorMessage(CreateProviderAllowedCourseCommandValidator.ExistsInProviderAllowedCourse);
    }

    [Test, MoqAutoData]
    public async Task WhenNotExistInProviderAllowedCourse_ThenValidationShouldPass(
        [Frozen] Mock<IStandardsReadRepository> standardsReadRepository,
        [Frozen] Mock<IProviderAllowedCoursesRepository> providerAllowedCoursesRepository,
        [Greedy] CreateProviderAllowedCourseCommandValidator sut)
    {
        // Arrange
        var command = new CreateProviderAllowedCourseCommand
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
}
