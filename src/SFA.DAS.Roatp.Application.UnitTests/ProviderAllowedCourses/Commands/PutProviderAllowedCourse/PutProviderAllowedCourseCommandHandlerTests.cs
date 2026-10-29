using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit4;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.ProviderAllowedCourses.Commands.PutProviderAllowedCourse;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.Application.UnitTests.ProviderAllowedCourses.Commands.PutProviderAllowedCourse;

public class PutProviderAllowedCourseCommandHandlerTests
{
    [Test, MoqAutoData]
    public async Task WhenHandlingCommand_ThenVerifyRepositoriesAreInvokedCorrectly(
        [Frozen] Mock<IProviderAllowedCoursesRepository> providerAllowedCoursesRepository,
        [Greedy] PutProviderAllowedCourseCommandHandler sut,
        PutProviderAllowedCourseCommand command)
    {
        // Act
        await sut.Handle(command, CancellationToken.None);

        // Assert
        var expectedDate = command.LastDateStarts?.Date;
        providerAllowedCoursesRepository.Verify(x => x.PutProviderAllowedCourse(
            command.Ukprn,
            command.LarsCode,
            expectedDate,
            command.UserId,
            command.UserDisplayName), Times.Once);
    }

    [Test, MoqAutoData]
    public async Task WhenHandlingCommand_ThenValidatedResponseIsReturned(
        [Frozen] Mock<IStandardsReadRepository> standardsReadRepository,
        [Frozen] Mock<IProviderAllowedCoursesRepository> providerAllowedCoursesRepository,
        [Greedy] PutProviderAllowedCourseCommandHandler sut,
        PutProviderAllowedCourseCommand command)
    {
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
