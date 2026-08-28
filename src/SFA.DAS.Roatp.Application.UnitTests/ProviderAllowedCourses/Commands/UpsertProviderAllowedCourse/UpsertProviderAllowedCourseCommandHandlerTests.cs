using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit4;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.ProviderAllowedCourses.Commands.UpsertProviderAllowedCourse;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Roatp.Domain.Models;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.Application.UnitTests.ProviderAllowedCourses.Commands.UpsertProviderAllowedCourse;

public class UpsertProviderAllowedCourseCommandHandlerTests
{
    [Test, MoqAutoData]
    public async Task WhenHandlingCommand_ThenVerifyRepositoriesAreInvokedCorrectly(
        [Frozen] Mock<IStandardsReadRepository> standardsReadRepository,
        [Frozen] Mock<IProviderAllowedCoursesRepository> providerAllowedCoursesRepository,
        [Greedy] UpsertProviderAllowedCourseCommandHandler sut,
        UpsertProviderAllowedCourseCommand command)
    {
        // Arrange
        var standard = new Standard { CourseType = CourseType.Apprenticeship };

        standardsReadRepository
            .Setup(x => x.GetStandard(command.LarsCode))
            .ReturnsAsync(standard);

        providerAllowedCoursesRepository
            .Setup(x => x.UpsertProviderAllowedCourse(command.Ukprn, command.LarsCode, command.IsStartRestricted, standard.CourseType, command.LastDateStarts, command.UserId, command.UserDisplayName))
            .Returns(Task.CompletedTask);

        // Act
        await sut.Handle(command, CancellationToken.None);

        // Assert
        standardsReadRepository.Verify(x => x.GetStandard(command.LarsCode), Times.Once);
        providerAllowedCoursesRepository.Verify(x => x.UpsertProviderAllowedCourse(
            command.Ukprn,
            command.LarsCode,
            command.IsStartRestricted,
            standard.CourseType,
            command.LastDateStarts,
            command.UserId,
            command.UserDisplayName), Times.Once);
    }

    [Test, MoqAutoData]
    public async Task WhenHandlingCommand_ThenValidatedResponseIsReturned(
        [Frozen] Mock<IStandardsReadRepository> standardsReadRepository,
        [Frozen] Mock<IProviderAllowedCoursesRepository> providerAllowedCoursesRepository,
        [Greedy] UpsertProviderAllowedCourseCommandHandler sut,
        UpsertProviderAllowedCourseCommand command)
    {
        // Arrange
        var standard = new Standard { CourseType = CourseType.Apprenticeship };

        standardsReadRepository
            .Setup(x => x.GetStandard(command.LarsCode))
            .ReturnsAsync(standard);

        providerAllowedCoursesRepository
            .Setup(x => x.UpsertProviderAllowedCourse(command.Ukprn, command.LarsCode, command.IsStartRestricted, standard.CourseType, command.LastDateStarts, command.UserId, command.UserDisplayName))
            .Returns(Task.CompletedTask);

        // Act
        var result = await sut.Handle(command, CancellationToken.None);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.IsValidResponse, Is.True);
            Assert.That(result.Result, Is.EqualTo(Unit.Value));
        });
    }
}
