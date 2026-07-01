using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit4;
using FluentValidation.TestHelper;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Application.RestrictedCourses.Commands.AddRestrictedCourse;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.Application.UnitTests.RestrictedCourses.Commands.AddRestrictedCourse;

public class AddRestrictedCourseCommandValidatorTests
{
    [Test]
    [MoqAutoData]
    public async Task WhenLarsCodeIsAlreadyRestricted_ThenValidationShouldFail(
        [Frozen] Mock<IStandardsReadRepository> standardsReadRepository,
        [Frozen] Mock<IRestrictedCourseViewRepository> restrictedCourseViewRepository,
        [Greedy] AddRestrictedCourseCommandValidator sut)
    {
        // Arrange
        var command = new AddRestrictedCourseCommand
        {
            LarsCode = "12345",
            UserId = "TestUserId",
            UserDisplayName = "TestUser"
        };

        standardsReadRepository
            .Setup(r => r.GetStandard(It.IsAny<string>()))
            .ReturnsAsync(new Standard { LarsCode = command.LarsCode });

        restrictedCourseViewRepository
            .Setup(x => x.GetRestrictedCourses(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<RestrictedCourseView>
            {
                new() { LarsCode = command.LarsCode }
            });

        // Act
        var result = await sut.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.LarsCode)
            .WithErrorMessage(AddRestrictedCourseCommandValidator.LarsCodeAlreadyRestricted);
    }

    [Test]
    [MoqAutoData]
    public async Task WhenLarsCodeDoesNotExist_ThenValidationShouldFail(
    [Frozen] Mock<IStandardsReadRepository> standardsReadRepository,
    [Frozen] Mock<IRestrictedCourseViewRepository> restrictedCourseViewRepository,
    [Greedy] AddRestrictedCourseCommandValidator sut)
    {
        // Arrange
        var command = new AddRestrictedCourseCommand
        {
            LarsCode = "12345",
            UserId = "TestUserId",
            UserDisplayName = "TestUser"
        };

        standardsReadRepository
            .Setup(r => r.GetStandard(It.IsAny<string>()))
            .ReturnsAsync((Standard)null);

        restrictedCourseViewRepository
            .Setup(x => x.GetRestrictedCourses(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<RestrictedCourseView>());

        // Act
        var result = await sut.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.LarsCode)
            .WithErrorMessage(LarsCodeValidator.NotFoundMessage);
    }

    [Test]
    [MoqAutoData]
    public async Task WhenLarsCodeIsNotRestricted_ThenValidationShouldPass(
        [Frozen] Mock<IStandardsReadRepository> standardsReadRepository,
        [Frozen] Mock<IRestrictedCourseViewRepository> restrictedCourseViewRepository,
        [Greedy] AddRestrictedCourseCommandValidator sut)
    {
        // Arrange
        var command = new AddRestrictedCourseCommand
        {
            LarsCode = "12345",
            UserId = "TestUserId",
            UserDisplayName = "TestUser"
        };

        standardsReadRepository
            .Setup(r => r.GetStandard(It.IsAny<string>()))
            .ReturnsAsync(new Standard { LarsCode = command.LarsCode });

        restrictedCourseViewRepository
            .Setup(x => x.GetRestrictedCourses(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<RestrictedCourseView>());

        // Act
        var result = await sut.TestValidateAsync(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.LarsCode);
    }
}
