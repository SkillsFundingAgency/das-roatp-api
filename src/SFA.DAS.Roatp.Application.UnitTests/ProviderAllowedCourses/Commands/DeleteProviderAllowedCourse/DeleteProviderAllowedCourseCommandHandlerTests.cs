using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit4;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.ProviderAllowedCourses.Commands.DeleteProviderAllowedCourse;
using SFA.DAS.Roatp.Domain.Constants;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.Application.UnitTests.ProviderAllowedCourses.Commands.DeleteProviderAllowedCourse;

public class DeleteProviderAllowedCourseCommandHandlerTests
{
    [Test, MoqAutoData]
    public async Task WhenHandlingCommand_ThenProviderAllowedCourseIsDeleted(
        [Frozen] Mock<IProviderAllowedCoursesRepository> providerAllowedCoursesRepository,
        [Greedy] DeleteProviderAllowedCourseCommandHandler sut,
        DeleteProviderAllowedCourseCommand command)
    {
        // Arrange
        providerAllowedCoursesRepository
            .Setup(x => x.DeleteProviderAllowedCourse(command.Ukprn, command.LarsCode, command.UserId, command.UserDisplayName, AuditEventTypes.DeleteProviderAllowedCourse))
            .Returns(Task.CompletedTask);

        // Act
        await sut.Handle(command, CancellationToken.None);

        // Assert
        providerAllowedCoursesRepository.Verify(
            x => x.DeleteProviderAllowedCourse(
                command.Ukprn,
                command.LarsCode,
                command.UserId,
                command.UserDisplayName,
                AuditEventTypes.DeleteProviderAllowedCourse),
            Times.Once);
    }
}
