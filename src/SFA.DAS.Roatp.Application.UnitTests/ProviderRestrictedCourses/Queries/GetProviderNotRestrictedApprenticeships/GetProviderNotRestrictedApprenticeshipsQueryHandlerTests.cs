using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit4;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.ProviderRestrictedCourses.Queries.GetProviderNotRestrictedApprenticeships;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.Application.UnitTests.ProviderRestrictedCourses.Queries.GetProviderNotRestrictedApprenticeships;

public class GetProviderNotRestrictedApprenticeshipsQueryHandlerTests
{
    [Test, MoqAutoData]
    public async Task WhenNonRestrictedCourseAndDoesNotExistInProviderAllowedCourses_ThenCourseIsReturned(
        [Frozen] Mock<IStandardsReadRepository> standardsReadRepository,
        [Frozen] Mock<IProviderAllowedCoursesRepository> providerAllowedCoursesRepository,
        [Greedy] GetProviderNotRestrictedApprenticeshipsQueryHandler sut,
        GetProviderNotRestrictedApprenticeshipsQuery request,
        string larsCode)
    {
        // Arrange
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
            .Setup(x => x.GetCoursesByCourseType(request.CourseType, It.IsAny<CancellationToken>()))
            .ReturnsAsync(standards);

        providerAllowedCoursesRepository
            .Setup(x => x.GetProviderAllowedCourses(request.Ukprn, request.CourseType, It.IsAny<CancellationToken>()))
            .ReturnsAsync(providerAllowedCourses);

        // Act
        var result = await sut.Handle(request, CancellationToken.None);

        // Assert
        result.Result.Courses[0].LarsCode.Should().Be(larsCode);
    }

    [Test, MoqAutoData]
    public async Task WhenNonRestrictedCourseAndExistsInProviderAllowedCoursesWithNullLastDateStarts_ThenCourseIsReturned(
        [Frozen] Mock<IStandardsReadRepository> standardsReadRepository,
        [Frozen] Mock<IProviderAllowedCoursesRepository> providerAllowedCoursesRepository,
        [Greedy] GetProviderNotRestrictedApprenticeshipsQueryHandler sut,
        GetProviderNotRestrictedApprenticeshipsQuery request,
        string larsCode)
    {
        // Arrange
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
                LarsCode = larsCode,
                LastDateStarts = null,
                Ukprn = request.Ukprn
            }
        };

        standardsReadRepository
            .Setup(x => x.GetCoursesByCourseType(request.CourseType, It.IsAny<CancellationToken>()))
            .ReturnsAsync(standards);

        providerAllowedCoursesRepository
            .Setup(x => x.GetProviderAllowedCourses(request.Ukprn, request.CourseType, It.IsAny<CancellationToken>()))
            .ReturnsAsync(providerAllowedCourses);

        // Act
        var result = await sut.Handle(request, CancellationToken.None);

        // Assert
        result.Result.Courses[0].LarsCode.Should().Be(larsCode);
    }

    [Test, MoqAutoData]
    public async Task WhenNonRestrictedCourseAndExistsInProviderAllowedCoursesWithLastDateStarts_ThenCourseIsNotReturned(
        [Frozen] Mock<IStandardsReadRepository> standardsReadRepository,
        [Frozen] Mock<IProviderAllowedCoursesRepository> providerAllowedCoursesRepository,
        [Greedy] GetProviderNotRestrictedApprenticeshipsQueryHandler sut,
        GetProviderNotRestrictedApprenticeshipsQuery request,
        string larsCode,
        DateTime lastDateStarts)
    {
        // Arrange
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
                LarsCode = larsCode,
                LastDateStarts = lastDateStarts,
                Ukprn = request.Ukprn
            }
        };

        standardsReadRepository
            .Setup(x => x.GetCoursesByCourseType(request.CourseType, It.IsAny<CancellationToken>()))
            .ReturnsAsync(standards);

        providerAllowedCoursesRepository
            .Setup(x => x.GetProviderAllowedCourses(request.Ukprn, request.CourseType, It.IsAny<CancellationToken>()))
            .ReturnsAsync(providerAllowedCourses);

        // Act
        var result = await sut.Handle(request, CancellationToken.None);

        // Assert
        result.Result.Courses.Should().BeEmpty();
    }

    [Test, MoqAutoData]
    public async Task WhenRestrictedCourseAndExistsInProviderAllowedCoursesWithNullLastDateStarts_ThenCourseIsReturned(
        [Frozen] Mock<IStandardsReadRepository> standardsReadRepository,
        [Frozen] Mock<IProviderAllowedCoursesRepository> providerAllowedCoursesRepository,
        [Greedy] GetProviderNotRestrictedApprenticeshipsQueryHandler sut,
        GetProviderNotRestrictedApprenticeshipsQuery request,
        string larsCode)
    {
        // Arrange
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
                LarsCode = larsCode,
                LastDateStarts = null,
                Ukprn = request.Ukprn
            }
        };

        standardsReadRepository
            .Setup(x => x.GetCoursesByCourseType(request.CourseType, It.IsAny<CancellationToken>()))
            .ReturnsAsync(standards);

        providerAllowedCoursesRepository
            .Setup(x => x.GetProviderAllowedCourses(request.Ukprn, request.CourseType, It.IsAny<CancellationToken>()))
            .ReturnsAsync(providerAllowedCourses);

        // Act
        var result = await sut.Handle(request, CancellationToken.None);

        // Assert
        result.Result.Courses[0].LarsCode.Should().Be(larsCode);
    }

    [Test, MoqAutoData]
    public async Task WhenRestrictedCourseAndExistsInProviderAllowedCoursesWithLastDateStarts_ThenCourseIsNotReturned(
        [Frozen] Mock<IStandardsReadRepository> standardsReadRepository,
        [Frozen] Mock<IProviderAllowedCoursesRepository> providerAllowedCoursesRepository,
        [Greedy] GetProviderNotRestrictedApprenticeshipsQueryHandler sut,
        GetProviderNotRestrictedApprenticeshipsQuery request,
        string larsCode,
        DateTime lastDateStarts)
    {
        // Arrange
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
                LarsCode = larsCode,
                LastDateStarts = lastDateStarts,
                Ukprn = request.Ukprn
            }
        };

        standardsReadRepository
            .Setup(x => x.GetCoursesByCourseType(request.CourseType, It.IsAny<CancellationToken>()))
            .ReturnsAsync(standards);

        providerAllowedCoursesRepository
            .Setup(x => x.GetProviderAllowedCourses(request.Ukprn, request.CourseType, It.IsAny<CancellationToken>()))
            .ReturnsAsync(providerAllowedCourses);

        // Act
        var result = await sut.Handle(request, CancellationToken.None);

        // Assert
        result.Result.Courses.Should().BeEmpty();
    }

    [Test, MoqAutoData]
    public async Task WhenRestrictedCourseAndDoesNotExistInProviderAllowedCourses_ThenCourseIsNotReturned(
        [Frozen] Mock<IStandardsReadRepository> standardsReadRepository,
        [Frozen] Mock<IProviderAllowedCoursesRepository> providerAllowedCoursesRepository,
        [Greedy] GetProviderNotRestrictedApprenticeshipsQueryHandler sut,
        GetProviderNotRestrictedApprenticeshipsQuery request,
        string larsCode)
    {
        // Arrange
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
            .Setup(x => x.GetCoursesByCourseType(request.CourseType, It.IsAny<CancellationToken>()))
            .ReturnsAsync(standards);

        providerAllowedCoursesRepository
            .Setup(x => x.GetProviderAllowedCourses(request.Ukprn, request.CourseType, It.IsAny<CancellationToken>()))
            .ReturnsAsync(providerAllowedCourses);

        // Act
        var result = await sut.Handle(request, CancellationToken.None);

        // Assert
        result.Result.Courses.Should().BeEmpty();
    }

    [Test, MoqAutoData]
    public async Task WhenRestrictedCourseAndProviderAllowedCourseHasDifferentLarsCode_ThenCourseIsNotReturned(
        [Frozen] Mock<IStandardsReadRepository> standardsReadRepository,
        [Frozen] Mock<IProviderAllowedCoursesRepository> providerAllowedCoursesRepository,
        [Greedy] GetProviderNotRestrictedApprenticeshipsQueryHandler sut,
        GetProviderNotRestrictedApprenticeshipsQuery request,
        string larsCode,
        string differentLarsCode)
    {
        // Arrange
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
                LarsCode = differentLarsCode,
                LastDateStarts = null,
                Ukprn = request.Ukprn
            }
        };

        standardsReadRepository
            .Setup(x => x.GetCoursesByCourseType(request.CourseType, It.IsAny<CancellationToken>()))
            .ReturnsAsync(standards);

        providerAllowedCoursesRepository
            .Setup(x => x.GetProviderAllowedCourses(request.Ukprn, request.CourseType, It.IsAny<CancellationToken>()))
            .ReturnsAsync(providerAllowedCourses);

        // Act
        var result = await sut.Handle(request, CancellationToken.None);

        // Assert
        result.Result.Courses.Should().BeEmpty();
    }
}
