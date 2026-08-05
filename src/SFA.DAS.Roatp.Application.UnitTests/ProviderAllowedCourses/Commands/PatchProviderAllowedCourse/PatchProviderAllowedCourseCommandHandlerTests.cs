using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit4;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.ProviderAllowedCourses.Commands.PatchProviderAllowedCourse;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.Application.UnitTests.ProviderAllowedCourses.Commands.PatchProviderAllowedCourse;

public class PatchProviderAllowedCourseCommandHandlerTests
{
    [Test, MoqAutoData]
    public async Task WhenHandlingCommand_ThenVerifyRepositoriesAreInvokedCorrectly(
        [Frozen] Mock<IStandardsReadRepository> standardsReadRepository,
        [Frozen] Mock<IProviderAllowedCoursesRepository> providerAllowedCoursesRepository,
        [Greedy] PatchProviderAllowedCourseCommandHandler sut,
        PatchProviderAllowedCourseCommand command)
    {
        // Arrange
        providerAllowedCoursesRepository
            .Setup(x => x.UpdateLastDateStarts(command.Ukprn, command.LarsCode, command.LastDateStarts, command.UserId, command.UserDisplayName))
            .Returns(Task.CompletedTask);

        // Act
        await sut.Handle(command, CancellationToken.None);

        // Assert
        providerAllowedCoursesRepository.Verify(x => x.UpdateLastDateStarts(
            command.Ukprn,
            command.LarsCode,
            command.LastDateStarts,
            command.UserId,
            command.UserDisplayName), Times.Once);
    }

    [Test, MoqAutoData]
    public async Task WhenHandlingCommand_ThenValidatedResponseIsReturned(
        [Frozen] Mock<IStandardsReadRepository> standardsReadRepository,
        [Frozen] Mock<IProviderAllowedCoursesRepository> providerAllowedCoursesRepository,
        [Greedy] PatchProviderAllowedCourseCommandHandler sut,
        PatchProviderAllowedCourseCommand command)
    {
        // Arrange
        providerAllowedCoursesRepository
            .Setup(x => x.UpdateLastDateStarts(command.Ukprn, command.LarsCode, command.LastDateStarts, command.UserId, command.UserDisplayName))
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
