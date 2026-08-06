using System;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit4;
using MediatR;
using Microsoft.AspNetCore.JsonPatch;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.ProviderAllowedCourses.Commands.PatchProviderAllowedCourse;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Roatp.Domain.Models;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.Application.UnitTests.ProviderAllowedCourses.Commands.PatchProviderAllowedCourse;

public class PatchProviderAllowedCourseCommandHandlerTests
{
    [Test, MoqAutoData]
    public async Task WhenHandlingCommand_ThenVerifyRepositoriesAreInvokedCorrectly(
        [Frozen] Mock<IStandardsReadRepository> standardsReadRepository,
        [Frozen] Mock<IProviderAllowedCoursesRepository> providerAllowedCoursesRepository,
        [Greedy] PatchProviderAllowedCourseCommandHandler sut)
    {
        // Arrange
        var expectedLastDateStarts = DateTime.UtcNow;

        var command = CreateCommand(expectedLastDateStarts);

        providerAllowedCoursesRepository
            .Setup(x => x.UpdateLastDateStarts(
                command.Ukprn,
                command.LarsCode,
                expectedLastDateStarts,
                command.UserId,
                command.UserDisplayName))
            .Returns(Task.CompletedTask);

        // Act
        await sut.Handle(command, CancellationToken.None);

        // Assert
        providerAllowedCoursesRepository.Verify(x => x.UpdateLastDateStarts(
            command.Ukprn,
            command.LarsCode,
            expectedLastDateStarts,
            command.UserId,
            command.UserDisplayName), Times.Once);
    }

    [Test, MoqAutoData]
    public async Task WhenHandlingCommand_ThenValidatedResponseIsReturned(
        [Frozen] Mock<IStandardsReadRepository> standardsReadRepository,
        [Frozen] Mock<IProviderAllowedCoursesRepository> providerAllowedCoursesRepository,
        [Greedy] PatchProviderAllowedCourseCommandHandler sut)
    {
        // Arrange
        var command = CreateCommand(DateTime.UtcNow);

        providerAllowedCoursesRepository
            .Setup(x => x.UpdateLastDateStarts(
                command.Ukprn,
                command.LarsCode,
                It.IsAny<DateTime?>(),
                command.UserId,
                command.UserDisplayName))
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

    private static PatchProviderAllowedCourseCommand CreateCommand(DateTime? lastDateStarts)
    {
        var patchDoc = new JsonPatchDocument<PatchProviderAllowedCourseModel>();
        patchDoc.Replace(x => x.LastDateStarts, lastDateStarts);

        return new PatchProviderAllowedCourseCommand
        {
            Ukprn = 12345678,
            LarsCode = "12345",
            UserId = "TestUserId",
            UserDisplayName = "TestUser",
            PatchDoc = patchDoc
        };
    }
}
