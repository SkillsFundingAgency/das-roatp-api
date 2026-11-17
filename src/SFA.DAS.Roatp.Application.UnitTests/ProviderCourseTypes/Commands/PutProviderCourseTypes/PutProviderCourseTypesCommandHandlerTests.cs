using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit4;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.ProviderCourseTypes.Commands.PutProviderCourseTypes;
using SFA.DAS.Roatp.Domain.Constants;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Roatp.Domain.Models;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.Application.UnitTests.ProviderCourseTypes.Commands.PutProviderCourseTypes;

public class PutProviderCourseTypesCommandHandlerTests
{
    [Test, MoqAutoData]
    public async Task WhenHandlingCommand_ThenSyncsRequestedCourseTypes(
        [Frozen] Mock<IProviderCourseTypesRepository> providerCourseTypesRepository,
        [Greedy] PutProviderCourseTypesCommandHandler sut)
    {
        // Arrange
        var command = new PutProviderCourseTypesCommand
        {
            Ukprn = 12345678,
            CourseTypes = [CourseType.Apprenticeship, CourseType.ShortCourse],
            UserId = "TestUserId",
            UserDisplayName = "Test User"
        };

        // Act
        await sut.Handle(command, CancellationToken.None);

        // Assert
        providerCourseTypesRepository.Verify(x => x.SyncProviderCourseTypes(
            command.Ukprn,
            It.Is<System.Collections.Generic.IEnumerable<CourseType>>(courseTypes =>
                courseTypes.SequenceEqual(command.CourseTypes)),
            command.UserId,
            command.UserDisplayName,
            AuditEventTypes.UpdateProviderCourseType,
            CancellationToken.None), Times.Once);
    }
}
