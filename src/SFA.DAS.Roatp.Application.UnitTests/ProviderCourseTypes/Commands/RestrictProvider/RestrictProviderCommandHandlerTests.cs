using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit4;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.ProviderCourseTypes.Commands.RestrictProvider;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Roatp.Domain.Models;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.Application.UnitTests.ProviderCourseTypes.Commands.RestrictProvider;

public class RestrictProviderCommandHandlerTests
{
    [Test, MoqAutoData]
    public async Task WhenProviderCourseDoesNotExistInProviderAllowedCourses_ThenVerifyCourseIsIncludedInRestriction(
        RestrictProviderCommand command,
        [Frozen] Mock<IProviderCoursesReadRepository> providerCoursesReadRepository,
        [Frozen] Mock<IProviderAllowedCoursesRepository> providerAllowedCoursesRepository,
        [Frozen] Mock<IProviderCourseTypesRepository> providerCourseTypesRepository,
        [Greedy] RestrictProviderCommandHandler sut)
    {
        // Arrange
        command.Ukprn = 12345678;
        command.CourseType = CourseType.Apprenticeship;
        command.UserId = "TestUserId";
        command.UserDisplayName = "Test User";

        var providerCourses = new List<Domain.Entities.ProviderCourse>
        {
            new()
            {
                Ukprn = command.Ukprn,
                LarsCode = "LARS001",
                Standard = new Standard
                {
                    CourseType = command.CourseType
                },
                ProviderAllowedCourse = null
            },
            new()
            {
                Ukprn = command.Ukprn,
                LarsCode = "LARS002",
                Standard = new Standard
                {
                    CourseType = command.CourseType
                },
                ProviderAllowedCourse = null
            }
        };

        providerCoursesReadRepository
            .Setup(x => x.GetAllProviderCourses(command.Ukprn))
            .ReturnsAsync(providerCourses);

        providerAllowedCoursesRepository
            .Setup(x => x.GetProviderAllowedCourses(
                command.Ukprn,
                command.CourseType,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ProviderAllowedCourse>());

        // Act
        await sut.Handle(command, CancellationToken.None);

        // Assert
        providerCourseTypesRepository.Verify(x => x.RestrictProvider(
            command.Ukprn,
            command.CourseType,
            It.Is<List<ProviderAllowedCourse>>(courses =>
                courses.Count == 2 &&
                courses.Any(c =>
                    c.Ukprn == command.Ukprn &&
                    c.LarsCode == "LARS001") &&
                courses.Any(c =>
                    c.Ukprn == command.Ukprn &&
                    c.LarsCode == "LARS002")),
            It.Is<List<ProviderAllowedCourse>>(courses => courses.Count == 0),
            command.UserId,
            command.UserDisplayName,
            It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Test, MoqAutoData]
    public async Task WhenProviderCourseAlreadyExistsInProviderAllowedCourses_ThenVerifyCourseIsNotAdded(
        RestrictProviderCommand command,
        [Frozen] Mock<IProviderCoursesReadRepository> providerCoursesReadRepository,
        [Frozen] Mock<IProviderAllowedCoursesRepository> providerAllowedCoursesRepository,
        [Frozen] Mock<IProviderCourseTypesRepository> providerCourseTypesRepository,
        [Greedy] RestrictProviderCommandHandler sut)
    {
        // Arrange
        command.Ukprn = 12345678;
        command.CourseType = CourseType.Apprenticeship;

        var providerCourses = new List<Domain.Entities.ProviderCourse>
        {
            new()
            {
                Ukprn = command.Ukprn,
                LarsCode = "LARS001",
                Standard = new Standard
                {
                    CourseType = command.CourseType
                },
                ProviderAllowedCourse = new ProviderAllowedCourse
                {
                    Ukprn = command.Ukprn,
                    LarsCode = "LARS001"
                }
            },
            new()
            {
                Ukprn = command.Ukprn,
                LarsCode = "LARS002",
                Standard = new Standard
                {
                    CourseType = command.CourseType
                },
                ProviderAllowedCourse = null
            }
        };

        var providerAllowedCourses = new List<ProviderAllowedCourse>
        {
            new()
            {
                Ukprn = command.Ukprn,
                LarsCode = "LARS001"
            }
        };

        providerCoursesReadRepository
            .Setup(x => x.GetAllProviderCourses(command.Ukprn))
            .ReturnsAsync(providerCourses);

        providerAllowedCoursesRepository
            .Setup(x => x.GetProviderAllowedCourses(
                command.Ukprn,
                command.CourseType,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(providerAllowedCourses);

        // Act
        await sut.Handle(command, CancellationToken.None);

        // Assert
        providerCourseTypesRepository.Verify(x => x.RestrictProvider(
            command.Ukprn,
            command.CourseType,
            It.Is<List<ProviderAllowedCourse>>(courses =>
                courses.Count == 1 &&
                courses[0].Ukprn == command.Ukprn &&
                courses[0].LarsCode == "LARS002"),
            It.Is<List<ProviderAllowedCourse>>(courses => courses.Count == 0),
            command.UserId,
            command.UserDisplayName,
            It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Test, MoqAutoData]
    public async Task WhenProviderAllowedCourseDoesNotExistInProviderCourses_ThenVerifyCourseIsRemoved(
        RestrictProviderCommand command,
        [Frozen] Mock<IProviderCoursesReadRepository> providerCoursesReadRepository,
        [Frozen] Mock<IProviderAllowedCoursesRepository> providerAllowedCoursesRepository,
        [Frozen] Mock<IProviderCourseTypesRepository> providerCourseTypesRepository,
        [Greedy] RestrictProviderCommandHandler sut)
    {
        // Arrange
        command.Ukprn = 12345678;
        command.CourseType = CourseType.Apprenticeship;

        var providerCourses = new List<Domain.Entities.ProviderCourse>
        {
            new()
            {
                Ukprn = command.Ukprn,
                LarsCode = "LARS001",
                Standard = new Standard
                {
                    CourseType = command.CourseType
                },
                ProviderAllowedCourse = new ProviderAllowedCourse
                {
                    Ukprn = command.Ukprn,
                    LarsCode = "LARS001"
                }
            }
        };

        var providerAllowedCourses = new List<ProviderAllowedCourse>
        {
            new()
            {
                Id = 1,
                Ukprn = command.Ukprn,
                LarsCode = "LARS001"
            },
            new()
            {
                Id = 2,
                Ukprn = command.Ukprn,
                LarsCode = "LARS002"
            }
        };

        providerCoursesReadRepository
            .Setup(x => x.GetAllProviderCourses(command.Ukprn))
            .ReturnsAsync(providerCourses);

        providerAllowedCoursesRepository
            .Setup(x => x.GetProviderAllowedCourses(
                command.Ukprn,
                command.CourseType,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(providerAllowedCourses);

        // Act
        await sut.Handle(command, CancellationToken.None);

        // Assert
        providerCourseTypesRepository.Verify(x => x.RestrictProvider(
            command.Ukprn,
            command.CourseType,
            It.Is<List<ProviderAllowedCourse>>(courses => courses.Count == 0),
            It.Is<List<ProviderAllowedCourse>>(courses =>
                courses.Count == 1 &&
                courses[0].Id == 2 &&
                courses[0].Ukprn == command.Ukprn &&
                courses[0].LarsCode == "LARS002"),
            command.UserId,
            command.UserDisplayName,
            It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Test, MoqAutoData]
    public async Task WhenProviderAllowedCourseExistsAndProviderCourseExists_ThenVerifyCourseIsNotRemoved(
        RestrictProviderCommand command,
        [Frozen] Mock<IProviderCoursesReadRepository> providerCoursesReadRepository,
        [Frozen] Mock<IProviderAllowedCoursesRepository> providerAllowedCoursesRepository,
        [Frozen] Mock<IProviderCourseTypesRepository> providerCourseTypesRepository,
        [Greedy] RestrictProviderCommandHandler sut)
    {
        // Arrange
        command.Ukprn = 12345678;
        command.CourseType = CourseType.Apprenticeship;

        var providerCourses = new List<Domain.Entities.ProviderCourse>
        {
            new()
            {
                Ukprn = command.Ukprn,
                LarsCode = "LARS001",
                Standard = new Standard
                {
                    CourseType = command.CourseType
                },
                ProviderAllowedCourse = new ProviderAllowedCourse
                {
                    Ukprn = command.Ukprn,
                    LarsCode = "LARS001"
                }
            }
        };

        var providerAllowedCourses = new List<ProviderAllowedCourse>
        {
            new()
            {
                Id = 1,
                Ukprn = command.Ukprn,
                LarsCode = "LARS001"
            }
        };

        providerCoursesReadRepository
            .Setup(x => x.GetAllProviderCourses(command.Ukprn))
            .ReturnsAsync(providerCourses);

        providerAllowedCoursesRepository
            .Setup(x => x.GetProviderAllowedCourses(
                command.Ukprn,
                command.CourseType,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(providerAllowedCourses);

        // Act
        await sut.Handle(command, CancellationToken.None);

        // Assert
        providerCourseTypesRepository.Verify(x => x.RestrictProvider(
            command.Ukprn,
            command.CourseType,
            It.Is<List<ProviderAllowedCourse>>(courses => courses.Count == 0),
            It.Is<List<ProviderAllowedCourse>>(courses =>
                courses.Count == 0),
            command.UserId,
            command.UserDisplayName,
            It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Test, MoqAutoData]
    public async Task WhenProviderCourseHasDifferentCourseType_ThenVerifyCourseIsNotAdded(
        RestrictProviderCommand command,
        [Frozen] Mock<IProviderCoursesReadRepository> providerCoursesReadRepository,
        [Frozen] Mock<IProviderAllowedCoursesRepository> providerAllowedCoursesRepository,
        [Frozen] Mock<IProviderCourseTypesRepository> providerCourseTypesRepository,
        [Greedy] RestrictProviderCommandHandler sut)
    {
        // Arrange
        command.Ukprn = 12345678;
        command.CourseType = CourseType.Apprenticeship;

        var providerCourses = new List<Domain.Entities.ProviderCourse>
        {
            new()
            {
                Ukprn = command.Ukprn,
                LarsCode = "LARS001",
                Standard = new Standard
                {
                    CourseType = CourseType.ShortCourse
                },
                ProviderAllowedCourse = null
            }
        };

        providerCoursesReadRepository
            .Setup(x => x.GetAllProviderCourses(command.Ukprn))
            .ReturnsAsync(providerCourses);

        providerAllowedCoursesRepository
            .Setup(x => x.GetProviderAllowedCourses(
                command.Ukprn,
                command.CourseType,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ProviderAllowedCourse>());

        // Act
        await sut.Handle(command, CancellationToken.None);

        // Assert
        providerCourseTypesRepository.Verify(x => x.RestrictProvider(
            command.Ukprn,
            command.CourseType,
            It.Is<List<ProviderAllowedCourse>>(courses => courses.Count == 0),
            It.Is<List<ProviderAllowedCourse>>(courses => courses.Count == 0),
            command.UserId,
            command.UserDisplayName,
            It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Test, MoqAutoData]
    public async Task WhenHandlingRestrictProviderCommand_ThenValidatedResponseIsReturned(
        RestrictProviderCommand command,
        [Frozen] Mock<IProviderCoursesReadRepository> providerCoursesReadRepository,
        [Frozen] Mock<IProviderAllowedCoursesRepository> providerAllowedCoursesRepository,
        [Frozen] Mock<IProviderCourseTypesRepository> providerCourseTypesRepository,
        [Greedy] RestrictProviderCommandHandler sut)
    {
        // Arrange
        command.Ukprn = 12345678;
        command.CourseType = CourseType.Apprenticeship;
        command.UserId = "TestUserId";
        command.UserDisplayName = "Test User";

        providerCoursesReadRepository
            .Setup(x => x.GetAllProviderCourses(command.Ukprn))
            .ReturnsAsync(new List<Domain.Entities.ProviderCourse>());

        providerAllowedCoursesRepository
            .Setup(x => x.GetProviderAllowedCourses(
                command.Ukprn,
                command.CourseType,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ProviderAllowedCourse>());

        providerCourseTypesRepository
            .Setup(x => x.RestrictProvider(
                command.Ukprn,
                command.CourseType,
                It.IsAny<List<ProviderAllowedCourse>>(),
                It.IsAny<List<ProviderAllowedCourse>>(),
                command.UserId,
                command.UserDisplayName,
                It.IsAny<CancellationToken>()))
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