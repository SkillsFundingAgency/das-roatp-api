using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit4;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.ProviderCourseTypes.Queries.GetProviderCourseTypes;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Roatp.Domain.Models;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.Application.UnitTests.ProviderCourseTypes.Queries;

[TestFixture]
public class ProviderCourseTypesQueryHandlerTests
{
    [Test, MoqAutoData]
    public async Task WhenApprenticeshipProviderIsRestricted_ThenRestrictedCountIsNull(
        [Frozen] Mock<IProviderCourseTypesRepository> providerCourseTypesRepository,
        [Frozen] Mock<IProviderAllowedCoursesRepository> providerAllowedCoursesRepository,
        [Frozen] Mock<IRestrictedCourseViewRepository> restrictedCourseViewRepository,
        [Greedy] GetProviderCourseTypesQueryHandler sut,
        GetProviderCourseTypesQuery request)
    {
        // Arrange
        var providerCourseType = new ProviderCourseType
        {
            CourseType = CourseType.Apprenticeship,
            IsRestrictedProvider = true
        };

        var providerAllowedCourses = new List<ProviderAllowedCourse>();

        var restrictedCourses = new List<RestrictedCourseView>();

        providerCourseTypesRepository
            .Setup(x => x.GetProviderCourseTypesByUkprn(request.Ukprn, It.IsAny<CancellationToken>()))
            .ReturnsAsync([providerCourseType]);

        providerAllowedCoursesRepository
            .Setup(x => x.GetProviderAllowedCourses(request.Ukprn, CourseType.Apprenticeship, It.IsAny<CancellationToken>()))
            .ReturnsAsync(providerAllowedCourses);

        restrictedCourseViewRepository
            .Setup(x => x.GetRestrictedCourses(It.IsAny<CancellationToken>()))
            .ReturnsAsync(restrictedCourses);

        // Act
        var result = await sut.Handle(request, CancellationToken.None);

        // Assert
        result.Result[0].RestrictedCount.Should().BeNull();
    }

    [Test, MoqAutoData]
    public async Task WhenRestrictedApprenticeshipProviderHasCourseWithNullLastDateStarts_ThenCourseIsIncludedInAllowedCount(
        [Frozen] Mock<IProviderCourseTypesRepository> providerCourseTypesRepository,
        [Frozen] Mock<IProviderAllowedCoursesRepository> providerAllowedCoursesRepository,
        [Frozen] Mock<IRestrictedCourseViewRepository> restrictedCourseViewRepository,
        [Greedy] GetProviderCourseTypesQueryHandler sut,
        GetProviderCourseTypesQuery request)
    {
        // Arrange
        var providerCourseType = new ProviderCourseType
        {
            CourseType = CourseType.Apprenticeship,
            IsRestrictedProvider = true
        };

        var providerAllowedCourses = new List<ProviderAllowedCourse>
        {
            new()
            {
                LarsCode = "100",
                LastDateStarts = null
            }
        };

        var restrictedCourses = new List<RestrictedCourseView>();

        providerCourseTypesRepository
            .Setup(x => x.GetProviderCourseTypesByUkprn(request.Ukprn, It.IsAny<CancellationToken>()))
            .ReturnsAsync([providerCourseType]);

        providerAllowedCoursesRepository
            .Setup(x => x.GetProviderAllowedCourses(request.Ukprn, CourseType.Apprenticeship,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(providerAllowedCourses);

        restrictedCourseViewRepository
            .Setup(x => x.GetRestrictedCourses(It.IsAny<CancellationToken>()))
            .ReturnsAsync(restrictedCourses);

        // Act
        var result = await sut.Handle(request, CancellationToken.None);

        // Assert
        result.Result[0].AllowedCount.Should().Be(1);
    }

    [Test, MoqAutoData]
    public async Task WhenApprenticeshipProviderIsUnrestricted_ThenAllowedCountIsNull(
        [Frozen] Mock<IProviderCourseTypesRepository> providerCourseTypesRepository,
        [Frozen] Mock<IProviderAllowedCoursesRepository> providerAllowedCoursesRepository,
        [Frozen] Mock<IRestrictedCourseViewRepository> restrictedCourseViewRepository,
        [Greedy] GetProviderCourseTypesQueryHandler sut,
        GetProviderCourseTypesQuery request)
    {
        // Arrange
        var providerCourseType = new ProviderCourseType
        {
            CourseType = CourseType.Apprenticeship,
            IsRestrictedProvider = false
        };

        var providerAllowedCourses = new List<ProviderAllowedCourse>();

        var restrictedCourses = new List<RestrictedCourseView>();

        providerCourseTypesRepository
            .Setup(x => x.GetProviderCourseTypesByUkprn(request.Ukprn, It.IsAny<CancellationToken>()))
            .ReturnsAsync([providerCourseType]);

        providerAllowedCoursesRepository
            .Setup(x => x.GetProviderAllowedCourses(request.Ukprn, CourseType.Apprenticeship, It.IsAny<CancellationToken>()))
            .ReturnsAsync(providerAllowedCourses);

        restrictedCourseViewRepository
            .Setup(x => x.GetRestrictedCourses(It.IsAny<CancellationToken>()))
            .ReturnsAsync(restrictedCourses);

        // Act
        var result = await sut.Handle(request, CancellationToken.None);

        // Assert
        result.Result[0].AllowedCount.Should().BeNull();
    }

    [Test, MoqAutoData]
    public async Task WhenApprenticeshipProviderIsUnrestricted_ThenRestrictedCountIncludesAllApprenticeshipRestrictedCourses(
        [Frozen] Mock<IProviderCourseTypesRepository> providerCourseTypesRepository,
        [Frozen] Mock<IProviderAllowedCoursesRepository> providerAllowedCoursesRepository,
        [Frozen] Mock<IRestrictedCourseViewRepository> restrictedCourseViewRepository,
        [Greedy] GetProviderCourseTypesQueryHandler sut,
        GetProviderCourseTypesQuery request)
    {
        // Arrange
        var providerCourseType = new ProviderCourseType
        {
            CourseType = CourseType.Apprenticeship,
            IsRestrictedProvider = false
        };

        var providerAllowedCourses = new List<ProviderAllowedCourse>();

        var restrictedCourses = new List<RestrictedCourseView>
        {
            new()
            {
                LarsCode = "100",
                Standard = new()
                {
                    CourseType = CourseType.Apprenticeship
                }
            },
            new()
            {
                LarsCode = "200",
                Standard = new()
                {
                    CourseType = CourseType.Apprenticeship
                }
            }
        };

        providerCourseTypesRepository
            .Setup(x => x.GetProviderCourseTypesByUkprn(request.Ukprn, It.IsAny<CancellationToken>()))
            .ReturnsAsync([providerCourseType]);

        providerAllowedCoursesRepository
            .Setup(x => x.GetProviderAllowedCourses(request.Ukprn, CourseType.Apprenticeship, It.IsAny<CancellationToken>()))
            .ReturnsAsync(providerAllowedCourses);

        restrictedCourseViewRepository
            .Setup(x => x.GetRestrictedCourses(It.IsAny<CancellationToken>()))
            .ReturnsAsync(restrictedCourses);

        // Act
        var result = await sut.Handle(request, CancellationToken.None);

        // Assert
        result.Result[0].RestrictedCount.Should().Be(2);
    }

    [Test, MoqAutoData]
    public async Task WhenUnrestrictedApprenticeshipProviderHasNonRestrictedCourseWithLastDateStarts_ThenCourseIsAddedToRestrictedCount(
        [Frozen] Mock<IProviderCourseTypesRepository> providerCourseTypesRepository,
        [Frozen] Mock<IProviderAllowedCoursesRepository> providerAllowedCoursesRepository,
        [Frozen] Mock<IRestrictedCourseViewRepository> restrictedCourseViewRepository,
        [Greedy] GetProviderCourseTypesQueryHandler sut,
        GetProviderCourseTypesQuery request)
    {
        // Arrange
        var providerCourseType = new ProviderCourseType
        {
            CourseType = CourseType.Apprenticeship,
            IsRestrictedProvider = false
        };

        var providerAllowedCourses = new List<ProviderAllowedCourse>
        {
            new()
            {
                LarsCode = "100",
                LastDateStarts = DateTime.Today.AddDays(-1)
            }
        };

        var restrictedCourses = new List<RestrictedCourseView>
        {
            new()
            {
                LarsCode = "200",
                Standard = new()
                {
                    CourseType = CourseType.Apprenticeship
                }
            }
        };

        providerCourseTypesRepository
            .Setup(x => x.GetProviderCourseTypesByUkprn(request.Ukprn, It.IsAny<CancellationToken>()))
            .ReturnsAsync([providerCourseType]);

        providerAllowedCoursesRepository
            .Setup(x => x.GetProviderAllowedCourses(request.Ukprn, CourseType.Apprenticeship, It.IsAny<CancellationToken>()))
            .ReturnsAsync(providerAllowedCourses);

        restrictedCourseViewRepository
            .Setup(x => x.GetRestrictedCourses(It.IsAny<CancellationToken>()))
            .ReturnsAsync(restrictedCourses);

        // Act
        var result = await sut.Handle(request, CancellationToken.None);

        // Assert
        result.Result[0].RestrictedCount.Should().Be(2);
    }

    [Test, MoqAutoData]
    public async Task WhenUnrestrictedApprenticeshipProviderHasRestrictedCourseWithNullLastDateStarts_ThenCourseIsRemovedFromRestrictedCount(
        [Frozen] Mock<IProviderCourseTypesRepository> providerCourseTypesRepository,
        [Frozen] Mock<IProviderAllowedCoursesRepository> providerAllowedCoursesRepository,
        [Frozen] Mock<IRestrictedCourseViewRepository> restrictedCourseViewRepository,
        [Greedy] GetProviderCourseTypesQueryHandler sut,
        GetProviderCourseTypesQuery request)
    {
        // Arrange
        const string larsCode = "100";

        var providerCourseType = new ProviderCourseType
        {
            CourseType = CourseType.Apprenticeship,
            IsRestrictedProvider = false
        };

        var providerAllowedCourses = new List<ProviderAllowedCourse>
        {
            new()
            {
                LarsCode = larsCode,
                LastDateStarts = null
            }
        };

        var restrictedCourses = new List<RestrictedCourseView>
        {
            new()
            {
                LarsCode = larsCode,
                Standard = new()
                {
                    CourseType = CourseType.Apprenticeship
                }
            }
        };

        providerCourseTypesRepository
            .Setup(x => x.GetProviderCourseTypesByUkprn(request.Ukprn, It.IsAny<CancellationToken>()))
            .ReturnsAsync([providerCourseType]);

        providerAllowedCoursesRepository
            .Setup(x => x.GetProviderAllowedCourses(request.Ukprn, CourseType.Apprenticeship, It.IsAny<CancellationToken>()))
            .ReturnsAsync(providerAllowedCourses);

        restrictedCourseViewRepository
            .Setup(x => x.GetRestrictedCourses(It.IsAny<CancellationToken>()))
            .ReturnsAsync(restrictedCourses);

        // Act
        var result = await sut.Handle(request, CancellationToken.None);

        // Assert
        result.Result[0].RestrictedCount.Should().Be(0);
    }

    [Test, MoqAutoData]
    public async Task WhenCourseTypeIsShortCourse_ThenAllowedCountIsProviderAllowedCourseCount(
        [Frozen] Mock<IProviderCourseTypesRepository> providerCourseTypesRepository,
        [Frozen] Mock<IProviderAllowedCoursesRepository> providerAllowedCoursesRepository,
        [Frozen] Mock<IRestrictedCourseViewRepository> restrictedCourseViewRepository,
        [Greedy] GetProviderCourseTypesQueryHandler sut,
        GetProviderCourseTypesQuery request)
    {
        // Arrange
        var providerCourseType = new ProviderCourseType
        {
            CourseType = CourseType.ShortCourse
        };

        var providerAllowedCourses = new List<ProviderAllowedCourse>
        {
            new()
            {
                LarsCode = "100"
            },
            new()
            {
                LarsCode = "200"
            }
        };

        var restrictedCourses = new List<RestrictedCourseView>();

        providerCourseTypesRepository
            .Setup(x => x.GetProviderCourseTypesByUkprn(request.Ukprn, It.IsAny<CancellationToken>()))
            .ReturnsAsync([providerCourseType]);

        providerAllowedCoursesRepository
            .Setup(x => x.GetProviderAllowedCourses(request.Ukprn, CourseType.ShortCourse, It.IsAny<CancellationToken>()))
            .ReturnsAsync(providerAllowedCourses);

        restrictedCourseViewRepository
            .Setup(x => x.GetRestrictedCourses(It.IsAny<CancellationToken>()))
            .ReturnsAsync(restrictedCourses);

        // Act
        var result = await sut.Handle(request, CancellationToken.None);

        // Assert
        result.Result[0].AllowedCount.Should().Be(2);
    }

    [Test, MoqAutoData]
    public async Task WhenCourseTypeIsShortCourse_ThenRestrictedCountIsNull(
        [Frozen] Mock<IProviderCourseTypesRepository> providerCourseTypesRepository,
        [Frozen] Mock<IProviderAllowedCoursesRepository> providerAllowedCoursesRepository,
        [Frozen] Mock<IRestrictedCourseViewRepository> restrictedCourseViewRepository,
        [Greedy] GetProviderCourseTypesQueryHandler sut,
        GetProviderCourseTypesQuery request)
    {
        // Arrange
        var providerCourseType = new ProviderCourseType
        {
            CourseType = CourseType.ShortCourse
        };

        var providerAllowedCourses = new List<ProviderAllowedCourse>();

        var restrictedCourses = new List<RestrictedCourseView>();

        providerCourseTypesRepository
            .Setup(x => x.GetProviderCourseTypesByUkprn(request.Ukprn, It.IsAny<CancellationToken>()))
            .ReturnsAsync([providerCourseType]);

        providerAllowedCoursesRepository
            .Setup(x => x.GetProviderAllowedCourses(request.Ukprn, CourseType.ShortCourse, It.IsAny<CancellationToken>()))
            .ReturnsAsync(providerAllowedCourses);

        restrictedCourseViewRepository
            .Setup(x => x.GetRestrictedCourses(It.IsAny<CancellationToken>()))
            .ReturnsAsync(restrictedCourses);

        // Act
        var result = await sut.Handle(request, CancellationToken.None);

        // Assert
        result.Result[0].RestrictedCount.Should().BeNull();
    }
}