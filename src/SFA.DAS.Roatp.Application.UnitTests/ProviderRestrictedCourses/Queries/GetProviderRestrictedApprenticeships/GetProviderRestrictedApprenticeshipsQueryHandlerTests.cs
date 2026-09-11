using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit4;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.ProviderRestrictedCourses.Queries.GetProviderRestrictedApprenticeships;
using SFA.DAS.Roatp.Domain.Constants;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Roatp.Domain.Models;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.Application.UnitTests.ProviderRestrictedCourses.Queries.GetProviderRestrictedApprenticeships;

public class GetProviderRestrictedApprenticeshipsQueryHandlerTests
{
    [Test, MoqAutoData]
    public async Task WhenRestrictedCourseDoesNotExistInProviderAllowedCourse_ThenCourseIsReturned(
        [Frozen] Mock<IStandardsReadRepository> standardsReadRepository,
        [Frozen] Mock<IProviderAllowedCoursesRepository> providerAllowedCoursesRepository,
        [Greedy] GetProviderRestrictedApprenticeshipsQueryHandler sut,
        GetProviderRestrictedApprenticeshipsQuery request)
    {
        // Arrange
        const string larsCode = "100";

        var standards = new List<Standard>
        {
            new()
            {
                LarsCode = larsCode,
                RestrictedCourseView = new RestrictedCourseView()
            }
        };

        var providerAllowedCourses = new List<ProviderAllowedCourse>();

        standardsReadRepository
            .Setup(x => x.GetCoursesByCourseType(
                CourseType.Apprenticeship,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(standards);

        providerAllowedCoursesRepository
            .Setup(x => x.GetProviderAllowedCourses(
                request.Ukprn,
                CourseType.Apprenticeship,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(providerAllowedCourses);

        // Act
        var result = await sut.Handle(request, CancellationToken.None);

        // Assert
        result.Result.Courses.Should().ContainSingle();
        result.Result.Courses[0].LarsCode.Should().Be(larsCode);
        result.Result.Courses[0].LastDateStarts.Should().BeNull();
        result.Result.Courses[0].IsClosedToNewStarts.Should().BeTrue();
    }

    [Test, MoqAutoData]
    public async Task WhenRestrictedCourseExistsInProviderAllowedCourseWithNullLastDateStarts_ThenCourseIsNotReturned(
        [Frozen] Mock<IStandardsReadRepository> standardsReadRepository,
        [Frozen] Mock<IProviderAllowedCoursesRepository> providerAllowedCoursesRepository,
        [Greedy] GetProviderRestrictedApprenticeshipsQueryHandler sut,
        GetProviderRestrictedApprenticeshipsQuery request)
    {
        // Arrange
        const string larsCode = "100";

        var standards = new List<Standard>
        {
            new()
            {
                LarsCode = larsCode,
                RestrictedCourseView = new RestrictedCourseView()
            }
        };

        var providerAllowedCourses = new List<ProviderAllowedCourse>
        {
            new()
            {
                Ukprn = request.Ukprn,
                LarsCode = larsCode,
                LastDateStarts = null
            }
        };

        standardsReadRepository
            .Setup(x => x.GetCoursesByCourseType(
                CourseType.Apprenticeship,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(standards);

        providerAllowedCoursesRepository
            .Setup(x => x.GetProviderAllowedCourses(
                request.Ukprn,
                CourseType.Apprenticeship,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(providerAllowedCourses);

        // Act
        var result = await sut.Handle(request, CancellationToken.None);

        // Assert
        result.Result.Courses.Should().BeEmpty();
    }

    [Test, MoqAutoData]
    public async Task WhenNonRestrictedCourseExistsInProviderAllowedCourseWithLastDateStarts_ThenCourseIsReturned(
        [Frozen] Mock<IStandardsReadRepository> standardsReadRepository,
        [Frozen] Mock<IProviderAllowedCoursesRepository> providerAllowedCoursesRepository,
        [Greedy] GetProviderRestrictedApprenticeshipsQueryHandler sut,
        GetProviderRestrictedApprenticeshipsQuery request)
    {
        // Arrange
        const string larsCode = "100";
        var lastDateStarts = DateTime.UtcNow.Date.AddDays(1);

        var standards = new List<Standard>
        {
            new()
            {
                LarsCode = larsCode,
                RestrictedCourseView = null
            }
        };

        var providerAllowedCourses = new List<ProviderAllowedCourse>
        {
            new()
            {
                Ukprn = request.Ukprn,
                LarsCode = larsCode,
                LastDateStarts = lastDateStarts
            }
        };

        standardsReadRepository
            .Setup(x => x.GetCoursesByCourseType(
                CourseType.Apprenticeship,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(standards);

        providerAllowedCoursesRepository
            .Setup(x => x.GetProviderAllowedCourses(
                request.Ukprn,
                CourseType.Apprenticeship,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(providerAllowedCourses);

        // Act
        var result = await sut.Handle(request, CancellationToken.None);

        // Assert
        result.Result.Courses.Should().ContainSingle();
        result.Result.Courses[0].LarsCode.Should().Be(larsCode);
        result.Result.Courses[0].LastDateStarts.Should().Be(lastDateStarts);
    }

    [Test, MoqAutoData]
    public async Task WhenNonRestrictedCourseDoesNotExistInProviderAllowedCourse_ThenCourseIsNotReturned(
        [Frozen] Mock<IStandardsReadRepository> standardsReadRepository,
        [Frozen] Mock<IProviderAllowedCoursesRepository> providerAllowedCoursesRepository,
        [Greedy] GetProviderRestrictedApprenticeshipsQueryHandler sut,
        GetProviderRestrictedApprenticeshipsQuery request)
    {
        // Arrange
        const string larsCode = "100";

        var standards = new List<Standard>
        {
            new()
            {
                LarsCode = larsCode,
                RestrictedCourseView = null
            }
        };

        var providerAllowedCourses = new List<ProviderAllowedCourse>();

        standardsReadRepository
            .Setup(x => x.GetCoursesByCourseType(
                CourseType.Apprenticeship,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(standards);

        providerAllowedCoursesRepository
            .Setup(x => x.GetProviderAllowedCourses(
                request.Ukprn,
                CourseType.Apprenticeship,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(providerAllowedCourses);

        // Act
        var result = await sut.Handle(request, CancellationToken.None);

        // Assert
        result.Result.Courses.Should().BeEmpty();
    }

    [Test, MoqAutoData]
    public async Task WhenLastDateStartsIsStartRestrictedDate_ThenLastDateStartsIsNull(
        [Frozen] Mock<IStandardsReadRepository> standardsReadRepository,
        [Frozen] Mock<IProviderAllowedCoursesRepository> providerAllowedCoursesRepository,
        [Greedy] GetProviderRestrictedApprenticeshipsQueryHandler sut,
        GetProviderRestrictedApprenticeshipsQuery request)
    {
        // Arrange
        const string larsCode = "100";

        var standards = new List<Standard>
        {
            new()
            {
                LarsCode = larsCode,
                RestrictedCourseView = null
            }
        };

        var providerAllowedCourses = new List<ProviderAllowedCourse>
        {
            new()
            {
                Ukprn = request.Ukprn,
                LarsCode = larsCode,
                LastDateStarts = DateConstants.StartRestrictedDate
            }
        };

        standardsReadRepository
            .Setup(x => x.GetCoursesByCourseType(
                CourseType.Apprenticeship,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(standards);

        providerAllowedCoursesRepository
            .Setup(x => x.GetProviderAllowedCourses(
                request.Ukprn,
                CourseType.Apprenticeship,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(providerAllowedCourses);

        // Act
        var result = await sut.Handle(request, CancellationToken.None);

        // Assert
        result.Result.Courses.Should().ContainSingle();
        result.Result.Courses[0].LastDateStarts.Should().BeNull();
    }

    [Test, MoqAutoData]
    public async Task WhenLastDateStartsIsStartRestrictedDate_ThenIsClosedToNewStartsIsTrue(
        [Frozen] Mock<IStandardsReadRepository> standardsReadRepository,
        [Frozen] Mock<IProviderAllowedCoursesRepository> providerAllowedCoursesRepository,
        [Greedy] GetProviderRestrictedApprenticeshipsQueryHandler sut,
        GetProviderRestrictedApprenticeshipsQuery request)
    {
        // Arrange
        const string larsCode = "100";

        var standards = new List<Standard>
        {
            new()
            {
                LarsCode = larsCode,
                RestrictedCourseView = null
            }
        };

        var providerAllowedCourses = new List<ProviderAllowedCourse>
        {
            new()
            {
                Ukprn = request.Ukprn,
                LarsCode = larsCode,
                LastDateStarts = DateConstants.StartRestrictedDate
            }
        };

        standardsReadRepository
            .Setup(x => x.GetCoursesByCourseType(
                CourseType.Apprenticeship,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(standards);

        providerAllowedCoursesRepository
            .Setup(x => x.GetProviderAllowedCourses(
                request.Ukprn,
                CourseType.Apprenticeship,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(providerAllowedCourses);

        // Act
        var result = await sut.Handle(request, CancellationToken.None);

        // Assert
        result.Result.Courses.Should().ContainSingle();
        result.Result.Courses[0].IsClosedToNewStarts.Should().BeTrue();
    }

    [Test, MoqAutoData]
    public async Task WhenLastDateStartsIsInThePast_ThenLastDateStartsIsPopulated(
        [Frozen] Mock<IStandardsReadRepository> standardsReadRepository,
        [Frozen] Mock<IProviderAllowedCoursesRepository> providerAllowedCoursesRepository,
        [Greedy] GetProviderRestrictedApprenticeshipsQueryHandler sut,
        GetProviderRestrictedApprenticeshipsQuery request)
    {
        // Arrange
        const string larsCode = "100";
        var lastDateStarts = DateTime.UtcNow.Date.AddDays(-1);

        var standards = new List<Standard>
        {
            new()
            {
                LarsCode = larsCode,
                RestrictedCourseView = null
            }
        };

        var providerAllowedCourses = new List<ProviderAllowedCourse>
        {
            new()
            {
                Ukprn = request.Ukprn,
                LarsCode = larsCode,
                LastDateStarts = lastDateStarts
            }
        };

        standardsReadRepository
            .Setup(x => x.GetCoursesByCourseType(
                CourseType.Apprenticeship,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(standards);

        providerAllowedCoursesRepository
            .Setup(x => x.GetProviderAllowedCourses(
                request.Ukprn,
                CourseType.Apprenticeship,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(providerAllowedCourses);

        // Act
        var result = await sut.Handle(request, CancellationToken.None);

        // Assert
        result.Result.Courses.Should().ContainSingle();
        result.Result.Courses[0].LastDateStarts.Should().Be(lastDateStarts);
    }

    [Test, MoqAutoData]
    public async Task WhenLastDateStartsIsInThePast_ThenIsClosedToNewStartsIsTrue(
        [Frozen] Mock<IStandardsReadRepository> standardsReadRepository,
        [Frozen] Mock<IProviderAllowedCoursesRepository> providerAllowedCoursesRepository,
        [Greedy] GetProviderRestrictedApprenticeshipsQueryHandler sut,
        GetProviderRestrictedApprenticeshipsQuery request)
    {
        // Arrange
        const string larsCode = "100";

        var standards = new List<Standard>
        {
            new()
            {
                LarsCode = larsCode,
                RestrictedCourseView = null
            }
        };

        var providerAllowedCourses = new List<ProviderAllowedCourse>
        {
            new()
            {
                Ukprn = request.Ukprn,
                LarsCode = larsCode,
                LastDateStarts = DateTime.UtcNow.Date.AddDays(-1)
            }
        };

        standardsReadRepository
            .Setup(x => x.GetCoursesByCourseType(
                CourseType.Apprenticeship,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(standards);

        providerAllowedCoursesRepository
            .Setup(x => x.GetProviderAllowedCourses(
                request.Ukprn,
                CourseType.Apprenticeship,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(providerAllowedCourses);

        // Act
        var result = await sut.Handle(request, CancellationToken.None);

        // Assert
        result.Result.Courses.Should().ContainSingle();
        result.Result.Courses[0].IsClosedToNewStarts.Should().BeTrue();
    }

    [Test, MoqAutoData]
    public async Task WhenLastDateStartsIsToday_ThenIsClosedToNewStartsIsFalse(
        [Frozen] Mock<IStandardsReadRepository> standardsReadRepository,
        [Frozen] Mock<IProviderAllowedCoursesRepository> providerAllowedCoursesRepository,
        [Greedy] GetProviderRestrictedApprenticeshipsQueryHandler sut,
        GetProviderRestrictedApprenticeshipsQuery request)
    {
        // Arrange
        const string larsCode = "100";
        var lastDateStarts = DateTime.UtcNow.Date;

        var standards = new List<Standard>
        {
            new()
            {
                LarsCode = larsCode,
                RestrictedCourseView = null
            }
        };

        var providerAllowedCourses = new List<ProviderAllowedCourse>
        {
            new()
            {
                Ukprn = request.Ukprn,
                LarsCode = larsCode,
                LastDateStarts = lastDateStarts
            }
        };

        standardsReadRepository
            .Setup(x => x.GetCoursesByCourseType(
                CourseType.Apprenticeship,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(standards);

        providerAllowedCoursesRepository
            .Setup(x => x.GetProviderAllowedCourses(
                request.Ukprn,
                CourseType.Apprenticeship,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(providerAllowedCourses);

        // Act
        var result = await sut.Handle(request, CancellationToken.None);

        // Assert
        result.Result.Courses.Should().ContainSingle();
        result.Result.Courses[0].LastDateStarts.Should().Be(lastDateStarts);
        result.Result.Courses[0].IsClosedToNewStarts.Should().BeFalse();
    }

    [Test, MoqAutoData]
    public async Task WhenLastDateStartsIsInTheFuture_ThenIsClosedToNewStartsIsFalse(
        [Frozen] Mock<IStandardsReadRepository> standardsReadRepository,
        [Frozen] Mock<IProviderAllowedCoursesRepository> providerAllowedCoursesRepository,
        [Greedy] GetProviderRestrictedApprenticeshipsQueryHandler sut,
        GetProviderRestrictedApprenticeshipsQuery request)
    {
        // Arrange
        const string larsCode = "100";
        var lastDateStarts = DateTime.UtcNow.Date.AddDays(1);

        var standards = new List<Standard>
        {
            new()
            {
                LarsCode = larsCode,
                RestrictedCourseView = null
            }
        };

        var providerAllowedCourses = new List<ProviderAllowedCourse>
        {
            new()
            {
                Ukprn = request.Ukprn,
                LarsCode = larsCode,
                LastDateStarts = lastDateStarts
            }
        };

        standardsReadRepository
            .Setup(x => x.GetCoursesByCourseType(
                CourseType.Apprenticeship,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(standards);

        providerAllowedCoursesRepository
            .Setup(x => x.GetProviderAllowedCourses(
                request.Ukprn,
                CourseType.Apprenticeship,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(providerAllowedCourses);

        // Act
        var result = await sut.Handle(request, CancellationToken.None);

        // Assert
        result.Result.Courses.Should().ContainSingle();
        result.Result.Courses[0].LastDateStarts.Should().Be(lastDateStarts);
        result.Result.Courses[0].IsClosedToNewStarts.Should().BeFalse();
    }

    [Test, MoqAutoData]
    public async Task WhenNonRestrictedCourseHasNoProviderAllowedCourse_ThenCourseIsNotReturned(
        [Frozen] Mock<IStandardsReadRepository> standardsReadRepository,
        [Frozen] Mock<IProviderAllowedCoursesRepository> providerAllowedCoursesRepository,
        [Greedy] GetProviderRestrictedApprenticeshipsQueryHandler sut,
        GetProviderRestrictedApprenticeshipsQuery request)
    {
        // Arrange
        var standards = new List<Standard>
        {
            new()
            {
                LarsCode = "100",
                RestrictedCourseView = null
            }
        };

        var providerAllowedCourses = new List<ProviderAllowedCourse>();

        standardsReadRepository
            .Setup(x => x.GetCoursesByCourseType(
                CourseType.Apprenticeship,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(standards);

        providerAllowedCoursesRepository
            .Setup(x => x.GetProviderAllowedCourses(
                request.Ukprn,
                CourseType.Apprenticeship,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(providerAllowedCourses);

        // Act
        var result = await sut.Handle(request, CancellationToken.None);

        // Assert
        result.Result.Courses.Should().BeEmpty();
    }
}
