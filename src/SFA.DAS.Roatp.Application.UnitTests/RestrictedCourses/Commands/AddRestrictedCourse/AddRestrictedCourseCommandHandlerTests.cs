using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit4;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.RestrictedCourses.Commands.AddRestrictedCourse;
using SFA.DAS.Roatp.Domain.Constants;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.Application.UnitTests.RestrictedCourses.Commands.AddRestrictedCourse;

public class AddRestrictedCourseCommandHandlerTests
{
    [Test]
    [MoqAutoData]
    public async Task WhenHandlingAddRestrictedCourseCommand_ThenRestrictedCourseIsCreated(
        AddRestrictedCourseCommand command,
        [Frozen] Mock<IRestrictedCourseWriteRepository> restrictedCourseWriteRepository,
        [Greedy] AddRestrictedCourseCommandHandler sut)
    {
        // Act
        await sut.Handle(command, CancellationToken.None);

        // Assert
        restrictedCourseWriteRepository.Verify(x => x.CreateRestrictedCourse(
                command.LarsCode,
                command.UserId,
                command.UserDisplayName,
                AuditEventTypes.CreateRestrictedCourse), Times.Once);
    }
}
