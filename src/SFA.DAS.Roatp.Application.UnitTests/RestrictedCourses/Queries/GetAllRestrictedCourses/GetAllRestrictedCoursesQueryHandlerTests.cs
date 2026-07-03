using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit4;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.RestrictedCourses.Queries.GetAllRestrictedCourses;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.Application.UnitTests.RestrictedCourses.Queries.GetAllRestrictedCourses;

public class GetAllRestrictedCoursesQueryHandlerTests
{
    [Test, MoqAutoData]
    public async Task WhenHandlingRestrictedCoursesRequest_ThenReturnExpectedCourses(
        [Frozen] Mock<IRestrictedCourseViewRepository> restrictedCourseViewRepositoryMock,
        [Frozen] Mock<IStandardsReadRepository> standardsReadRepositoryMock
    )
    {
        // Arrange
        var allStandards = new List<Standard>
        {
            new Standard { LarsCode = "123", Title = "Standard 1", Level = 1 },
            new Standard { LarsCode = "456", Title = "Standard 2", Level = 2 },
            new Standard { LarsCode = "789", Title = "Standard 3", Level = 3 }
        };

        var restrictedCourses = new List<RestrictedCourseView>
        {
            new RestrictedCourseView { LarsCode = allStandards[0].LarsCode, Standard = allStandards[0] },
            new RestrictedCourseView { LarsCode = allStandards[1].LarsCode, Standard = allStandards[1] }
        };

        restrictedCourseViewRepositoryMock.Setup(r => r.GetRestrictedCourses(It.IsAny<CancellationToken>()))
            .ReturnsAsync(restrictedCourses);
        standardsReadRepositoryMock.Setup(s => s.GetAllStandards())
            .ReturnsAsync(allStandards);

        var sut = new GetAllRestrictedCoursesQueryHandler(restrictedCourseViewRepositoryMock.Object, standardsReadRepositoryMock.Object);

        // Act
        var result = await sut.Handle(new GetAllRestrictedCoursesQuery(true), CancellationToken.None);

        // Assert
        Assert.AreEqual(2, result.Courses.Count);
        Assert.IsTrue(result.Courses.Exists(c => c.LarsCode == allStandards[0].LarsCode));
        Assert.IsTrue(result.Courses.Exists(c => c.LarsCode == allStandards[1].LarsCode));
        Assert.IsFalse(result.Courses.Exists(c => c.LarsCode == allStandards[2].LarsCode));
    }

    [Test, MoqAutoData]
    public async Task WhenHandlingNonRestrictedCoursesRequest_ThenReturnExpectedCourses(
        [Frozen] Mock<IRestrictedCourseViewRepository> restrictedCourseViewRepositoryMock,
        [Frozen] Mock<IStandardsReadRepository> standardsReadRepositoryMock
    )
    {
        // Arrange
        var allStandards = new List<Standard>
        {
            new Standard { LarsCode = "123", Title = "Standard 1", Level = 1 },
            new Standard { LarsCode = "456", Title = "Standard 2", Level = 2 },
            new Standard { LarsCode = "789", Title = "Standard 3", Level = 3 }
        };

        var restrictedCourses = new List<RestrictedCourseView>
        {
            new RestrictedCourseView { LarsCode = allStandards[0].LarsCode, Standard = allStandards[0] },
            new RestrictedCourseView { LarsCode = allStandards[1].LarsCode, Standard = allStandards[1] }
        };

        restrictedCourseViewRepositoryMock.Setup(r => r.GetRestrictedCourses(It.IsAny<CancellationToken>()))
            .ReturnsAsync(restrictedCourses);
        standardsReadRepositoryMock.Setup(s => s.GetAllStandards())
            .ReturnsAsync(allStandards);

        var sut = new GetAllRestrictedCoursesQueryHandler(restrictedCourseViewRepositoryMock.Object, standardsReadRepositoryMock.Object);

        // Act
        var result = await sut.Handle(new GetAllRestrictedCoursesQuery(false), CancellationToken.None);

        // Assert
        Assert.AreEqual(1, result.Courses.Count);
        Assert.IsTrue(result.Courses.Exists(c => c.LarsCode == allStandards[2].LarsCode));
        Assert.IsFalse(result.Courses.Exists(c => c.LarsCode == allStandards[0].LarsCode));
        Assert.IsFalse(result.Courses.Exists(c => c.LarsCode == allStandards[1].LarsCode));
    }
}
