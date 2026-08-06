using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit4;
using FluentValidation.TestHelper;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.ProviderAllowedCourses.Commands.PatchProviderAllowedCourse;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.Application.UnitTests.ProviderAllowedCourses.Commands.PatchProviderAllowedCourse;

public class PatchProviderAllowedCourseCommandValidatorTests
{
    [Test, MoqAutoData]
    public async Task WhenDoesNotExistInProviderAllowedCourse_ThenValidationShouldFail(
        [Frozen] Mock<IProviderAllowedCoursesRepository> providerAllowedCoursesRepository,
        [Frozen] Mock<IStandardsReadRepository> standardsReadRepository,
        [Greedy] PatchProviderAllowedCourseCommandValidator sut)
    {
        // Arrange
        var command = new PatchProviderAllowedCourseCommand
        {
            Ukprn = 12345678,
            LarsCode = "12345",
            UserId = "TestUserId",
            UserDisplayName = "TestUser",
            LastDateStarts = DateTime.UtcNow
        };

        var providerAllowedCourses = new List<ProviderAllowedCourse>();

        providerAllowedCoursesRepository
            .Setup(r => r.GetProviderAllowedCoursesByLarsCode(command.LarsCode, It.IsAny<CancellationToken>()))
            .ReturnsAsync(providerAllowedCourses);

        standardsReadRepository
            .Setup(r => r.GetStandard(It.IsAny<string>()))
            .ReturnsAsync(new Standard { LarsCode = command.LarsCode, LastDateStarts = DateTime.UtcNow });

        // Act
        var result = await sut.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x)
            .WithErrorMessage(PatchProviderAllowedCourseCommandValidator.NotExistsInProviderAllowedCourse);
    }

    [Test, MoqAutoData]
    public async Task WhenLastDateStartsIsAfterStandardDate_ThenValidationShouldFail(
        [Frozen] Mock<IProviderAllowedCoursesRepository> providerAllowedCoursesRepository,
        [Frozen] Mock<IStandardsReadRepository> standardsReadRepository,
        [Greedy] PatchProviderAllowedCourseCommandValidator sut)
    {
        // Arrange
        var command = new PatchProviderAllowedCourseCommand
        {
            Ukprn = 12345678,
            LarsCode = "12345",
            UserId = "TestUserId",
            UserDisplayName = "TestUser",
            LastDateStarts = DateTime.UtcNow
        };

        var providerAllowedCourses = new List<ProviderAllowedCourse>
            {
                new ProviderAllowedCourse
                {
                    Ukprn = command.Ukprn,
                    LarsCode = command.LarsCode,
                    LastDateStarts = DateTime.UtcNow.AddMonths(-2)
                }
            };

        providerAllowedCoursesRepository
            .Setup(r => r.GetProviderAllowedCoursesByLarsCode(command.LarsCode, It.IsAny<CancellationToken>()))
            .ReturnsAsync(providerAllowedCourses);

        standardsReadRepository
            .Setup(r => r.GetStandard(It.IsAny<string>()))
            .ReturnsAsync(new Standard { LarsCode = command.LarsCode, LastDateStarts = DateTime.UtcNow.AddMonths(-1) });

        // Act
        var result = await sut.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x)
            .WithErrorMessage(PatchProviderAllowedCourseCommandValidator.InvalidLastDateStarts);
    }

    [Test, MoqAutoData]
    public async Task WhenLastDateStartsIsBeforeStandardDate_AndExistsInProviderAllowedCourse_ThenValidationShouldPass(
        [Frozen] Mock<IProviderAllowedCoursesRepository> providerAllowedCoursesRepository,
        [Frozen] Mock<IStandardsReadRepository> standardsReadRepository,
        [Greedy] PatchProviderAllowedCourseCommandValidator sut)
    {
        // Arrange
        var command = new PatchProviderAllowedCourseCommand
        {
            Ukprn = 12345678,
            LarsCode = "12345",
            UserId = "TestUserId",
            UserDisplayName = "TestUser",
            LastDateStarts = DateTime.UtcNow.AddMonths(-1)
        };

        var providerAllowedCourses = new List<ProviderAllowedCourse>
            {
                new ProviderAllowedCourse
                {
                    Ukprn = command.Ukprn,
                    LarsCode = command.LarsCode,
                    LastDateStarts = DateTime.UtcNow.AddMonths(-2)
                }
            };

        providerAllowedCoursesRepository
            .Setup(r => r.GetProviderAllowedCoursesByLarsCode(command.LarsCode, It.IsAny<CancellationToken>()))
            .ReturnsAsync(providerAllowedCourses);

        standardsReadRepository
            .Setup(r => r.GetStandard(It.IsAny<string>()))
            .ReturnsAsync(new Standard { LarsCode = command.LarsCode, LastDateStarts = DateTime.UtcNow });

        // Act
        var result = await sut.TestValidateAsync(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x);
    }

    [Test, MoqAutoData]
    public async Task WhenLastDateStartsIsNull_ThenValidationShouldPass(
        [Frozen] Mock<IProviderAllowedCoursesRepository> providerAllowedCoursesRepository,
        [Frozen] Mock<IStandardsReadRepository> standardsReadRepository,
        [Greedy] PatchProviderAllowedCourseCommandValidator sut)
    {
        // Arrange
        var command = new PatchProviderAllowedCourseCommand
        {
            Ukprn = 12345678,
            LarsCode = "12345",
            UserId = "TestUserId",
            UserDisplayName = "TestUser",
            LastDateStarts = null
        };

        var providerAllowedCourses = new List<ProviderAllowedCourse>
            {
                new ProviderAllowedCourse
                {
                    Ukprn = command.Ukprn,
                    LarsCode = command.LarsCode,
                    LastDateStarts = DateTime.UtcNow.AddMonths(-2)
                }
            };

        providerAllowedCoursesRepository
            .Setup(r => r.GetProviderAllowedCoursesByLarsCode(command.LarsCode, It.IsAny<CancellationToken>()))
            .ReturnsAsync(providerAllowedCourses);

        standardsReadRepository
            .Setup(r => r.GetStandard(It.IsAny<string>()))
            .ReturnsAsync(new Standard { LarsCode = command.LarsCode, LastDateStarts = DateTime.UtcNow });

        // Act
        var result = await sut.TestValidateAsync(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x);
    }
}
