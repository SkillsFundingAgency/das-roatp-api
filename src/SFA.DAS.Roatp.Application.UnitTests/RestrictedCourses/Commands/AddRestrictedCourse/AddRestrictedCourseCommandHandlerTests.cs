using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit4;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.RestrictedCourses.Commands.AddRestrictedCourse;
using SFA.DAS.Roatp.Domain.Constants;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.Application.UnitTests.RestrictedCourses.Commands.AddRestrictedCourse;

public class AddRestrictedCourseCommandHandlerTests
{
    [Test]
    [MoqAutoData]
    public async Task WhenDoesNotExistInProviderAllowedCourse_ThenVerifyUkprnsAreIncludedInRestrictedCourseCreation(
        AddRestrictedCourseCommand command,
        [Frozen] Mock<IRestrictedCourseWriteRepository> restrictedCourseWriteRepository,
        [Frozen] Mock<IProviderCoursesReadRepository> providerCoursesReadRepository,
        [Frozen] Mock<IProviderAllowedCoursesRepository> providerAllowedCoursesRepository,
        [Greedy] AddRestrictedCourseCommandHandler sut)
    {
        // Arrange
        command.LarsCode = "123456";
        command.UserId = "TestUserId";
        command.UserDisplayName = "TestUser";

        var providerCourses = new List<SFA.DAS.Roatp.Domain.Entities.ProviderCourse>
        {
            new SFA.DAS.Roatp.Domain.Entities.ProviderCourse
            {
                LarsCode = command.LarsCode,
                Provider = new Provider { Ukprn = 10001 }
            },
            new SFA.DAS.Roatp.Domain.Entities.ProviderCourse
            {
                LarsCode = command.LarsCode,
                Provider = new Provider { Ukprn = 10002 }
            }
        };

        var providerAllowedCourses = new List<ProviderAllowedCourse>();

        providerCoursesReadRepository
            .Setup(x => x.GetProviderCoursesByLarsCode(command.LarsCode))
            .ReturnsAsync(providerCourses);

        providerAllowedCoursesRepository
            .Setup(x => x.GetProviderAllowedCoursesByLarsCode(command.LarsCode, It.IsAny<CancellationToken>()))
            .ReturnsAsync(providerAllowedCourses);

        // Act
        await sut.Handle(command, CancellationToken.None);

        // Assert
        restrictedCourseWriteRepository.Verify(x => x.CreateRestrictedCourse(
                command.LarsCode,
                It.Is<RestrictedCourse>(rc =>
                    rc.LarsCode == command.LarsCode &&
                    rc.ProviderAllowedCourses.Count == 2 &&
                    rc.ProviderAllowedCourses[0].Ukprn == 10001 &&
                    rc.ProviderAllowedCourses[0].LarsCode == command.LarsCode &&
                    rc.ProviderAllowedCourses[1].Ukprn == 10002 &&
                    rc.ProviderAllowedCourses[1].LarsCode == command.LarsCode),
                command.UserId,
                command.UserDisplayName,
                AuditEventTypes.CreateRestrictedCourse), Times.Once);
    }

    [Test]
    [MoqAutoData]
    public async Task WhenExistsInProviderAllowedCourse_ThenVerifyUkprnNotIncludedInRestrictedCourseCreation(
        AddRestrictedCourseCommand command,
        [Frozen] Mock<IRestrictedCourseWriteRepository> restrictedCourseWriteRepository,
        [Frozen] Mock<IProviderCoursesReadRepository> providerCoursesReadRepository,
        [Frozen] Mock<IProviderAllowedCoursesRepository> providerAllowedCoursesRepository,
        [Greedy] AddRestrictedCourseCommandHandler sut)
    {
        // Arrange
        command.LarsCode = "123456";
        command.UserId = "TestUserId";
        command.UserDisplayName = "TestUser";

        var providerCourses = new List<SFA.DAS.Roatp.Domain.Entities.ProviderCourse>
        {
            new SFA.DAS.Roatp.Domain.Entities.ProviderCourse
            {
                LarsCode = command.LarsCode,
                Provider = new Provider { Ukprn = 10001 }
            },
            new SFA.DAS.Roatp.Domain.Entities.ProviderCourse
            {
                LarsCode = command.LarsCode,
                Provider = new Provider { Ukprn = 10002 }
            }
        };

        var providerAllowedCourses = new List<ProviderAllowedCourse>
        {
            new ProviderAllowedCourse
            {
                Ukprn = 10001,
                LarsCode = command.LarsCode
            }
        };

        providerCoursesReadRepository
            .Setup(x => x.GetProviderCoursesByLarsCode(command.LarsCode))
            .ReturnsAsync(providerCourses);

        providerAllowedCoursesRepository
            .Setup(x => x.GetProviderAllowedCoursesByLarsCode(command.LarsCode, It.IsAny<CancellationToken>()))
            .ReturnsAsync(providerAllowedCourses);

        // Act
        await sut.Handle(command, CancellationToken.None);

        // Assert
        restrictedCourseWriteRepository.Verify(x => x.CreateRestrictedCourse(
                command.LarsCode,
                It.Is<RestrictedCourse>(rc =>
                    rc.LarsCode == command.LarsCode &&
                    rc.ProviderAllowedCourses.Count == 1 &&
                    rc.ProviderAllowedCourses[0].Ukprn == 10002 &&
                    rc.ProviderAllowedCourses[0].LarsCode == command.LarsCode),
                command.UserId,
                command.UserDisplayName,
                AuditEventTypes.CreateRestrictedCourse), Times.Once);
    }

    [Test]
    [MoqAutoData]
    public async Task WhenHandlingAddRestrictedCourseCommand_ThenValidatedResponseIsReturned(
        AddRestrictedCourseCommand command,
        [Frozen] Mock<IRestrictedCourseWriteRepository> restrictedCourseWriteRepository,
        [Frozen] Mock<IProviderCoursesReadRepository> providerCoursesReadRepository,
        [Frozen] Mock<IProviderAllowedCoursesRepository> providerAllowedCoursesRepository,
        [Greedy] AddRestrictedCourseCommandHandler sut)
    {
        // Arrange
        command.LarsCode = "123456";
        command.UserId = "TestUserId";
        command.UserDisplayName = "TestUser";

        providerCoursesReadRepository
            .Setup(x => x.GetProviderCoursesByLarsCode(command.LarsCode))
            .ReturnsAsync(new List<SFA.DAS.Roatp.Domain.Entities.ProviderCourse>());

        providerAllowedCoursesRepository
            .Setup(x => x.GetProviderAllowedCoursesByLarsCode(command.LarsCode, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ProviderAllowedCourse>());

        restrictedCourseWriteRepository
            .Setup(x => x.CreateRestrictedCourse(command.LarsCode, It.IsAny<RestrictedCourse>(), command.UserId, command.UserDisplayName, AuditEventTypes.CreateRestrictedCourse))
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
