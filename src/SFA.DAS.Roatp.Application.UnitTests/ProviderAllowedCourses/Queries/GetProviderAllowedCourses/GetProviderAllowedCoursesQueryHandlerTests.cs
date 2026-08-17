using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit4;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.ProviderAllowedCourses.Queries.GetProviderAllowedCourses;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Roatp.Domain.Models;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.Application.UnitTests.ProviderAllowedCourses.Queries.GetProviderAllowedCourses;

public class GetProviderAllowedCoursesQueryHandlerTests
{
    [Test, MoqAutoData]
    public async Task WhenProividerCourseTypeDoesNotExist_ThenReturnsEmpty(
        [Frozen] Mock<IProviderCourseTypesRepository> providerCourseTypesReadRepository,
        GetProviderAllowedCoursesQueryHandler sut,
        CancellationToken cancellationToken)
    {
        // Arrange
        const int ukprn = 100001;

        var providerCourseTypes = new List<ProviderCourseType>
        {
            new()
            {
                CourseType = CourseType.Apprenticeship
            }
        };

        providerCourseTypesReadRepository
            .Setup(x => x.GetProviderCourseTypesByUkprn(ukprn, cancellationToken))
            .ReturnsAsync(providerCourseTypes);

        // Act
        var response = await sut.Handle(new GetProviderAllowedCoursesQuery(ukprn, CourseType.ShortCourse), cancellationToken);

        // Assert
        response.AllowedCourses.Should().BeEmpty();
    }

    [Test, MoqAutoData]
    public async Task WhenProviderIsRestricted_ThenReturnsCoursesFromProviderAllowedCourses(
        [Frozen] Mock<IProviderCourseTypesRepository> providerCourseTypesReadRepository,
        [Frozen] Mock<IProviderAllowedCoursesRepository> providerAllowedCoursesRepository,
        GetProviderAllowedCoursesQueryHandler sut,
        int ukprn,
        CancellationToken cancellationToken)
    {
        // Arrange
        var courseType = CourseType.Apprenticeship;

        var providerCourseType = new List<ProviderCourseType>
        {
            new()
            {
                CourseType = courseType,
                IsRestrictedProvider = true
            }
        };

        var allowedCourses = new List<ProviderAllowedCourse>
        {
            new()
            {
                LarsCode = "123456",
                Ukprn = ukprn,
                LastDateStarts = DateTime.UtcNow.Date,
                Standard = new Standard
                {
                    LarsCode = "123456",
                    Title = "Test Course",
                    Level = 2,
                    CourseType = courseType
                }
            }
        };

        providerCourseTypesReadRepository
            .Setup(x => x.GetProviderCourseTypesByUkprn(ukprn, cancellationToken))
            .ReturnsAsync(providerCourseType);

        providerAllowedCoursesRepository
            .Setup(x => x.GetProviderAllowedCourses(ukprn, courseType, cancellationToken))
            .ReturnsAsync(allowedCourses);

        // Act
        var response = await sut.Handle(new GetProviderAllowedCoursesQuery(ukprn, courseType), cancellationToken);

        // Assert

        response.AllowedCourses.First().LarsCode.Should().Be(allowedCourses[0].LarsCode);
        response.AllowedCourses.First().Title.Should().Be(allowedCourses[0].Standard.Title);
        response.AllowedCourses.First().Level.Should().Be(allowedCourses[0].Standard.Level);
        response.AllowedCourses.First().LastDateStarts.Should().Be(allowedCourses[0].LastDateStarts);
    }

    [Test, MoqAutoData]
    public async Task WhenProviderIsNotRestricted_ThenReturnsCoursesFromStandards(
        [Frozen] Mock<IProviderCourseTypesRepository> providerCourseTypesReadRepository,
        [Frozen] Mock<IStandardsReadRepository> standardsReadRepository,
        [Frozen] Mock<IProviderCoursesReadRepository> providerCoursesReadRepository,
        GetProviderAllowedCoursesQueryHandler sut,
        int ukprn,
        CancellationToken cancellationToken)
    {
        // Arrange
        var courseType = CourseType.Apprenticeship;

        var providerCourseType = new List<ProviderCourseType>
        {
            new()
            {
                CourseType = courseType,
                IsRestrictedProvider = false
            }
        };

        var standard = new Standard
        {
            LarsCode = "123456",
            Title = "Test Course",
            Level = 2,
            CourseType = courseType,
            RestrictedCourseView = null
        };

        providerCourseTypesReadRepository
            .Setup(x => x.GetProviderCourseTypesByUkprn(ukprn, cancellationToken))
            .ReturnsAsync(providerCourseType);

        standardsReadRepository
            .Setup(x => x.GetAllStandards())
            .ReturnsAsync(new List<Standard> { standard });

        providerCoursesReadRepository
            .Setup(x => x.GetAllProviderCourses(ukprn))
            .ReturnsAsync(new List<Domain.Entities.ProviderCourse>());

        // Act
        var response = await sut.Handle(new GetProviderAllowedCoursesQuery(ukprn, courseType), cancellationToken);

        // Assert
        response.AllowedCourses.First().LarsCode.Should().Be(standard.LarsCode);
        response.AllowedCourses.First().Title.Should().Be(standard.Title);
        response.AllowedCourses.First().Level.Should().Be(standard.Level);
        response.AllowedCourses.First().IsActive.Should().BeFalse();
    }

    [Test, MoqAutoData]
    public async Task WhenCourseIsRestrictedAndProviderAllowedCourseExists_ThenReturnsCourse(
        [Frozen] Mock<IProviderCourseTypesRepository> providerCourseTypesReadRepository,
        [Frozen] Mock<IStandardsReadRepository> standardsReadRepository,
        [Frozen] Mock<IProviderCoursesReadRepository> providerCoursesReadRepository,
        GetProviderAllowedCoursesQueryHandler sut,
        int ukprn,
        CancellationToken cancellationToken)
    {
        // Arrange
        var courseType = CourseType.Apprenticeship;

        var providerCourseType = new List<ProviderCourseType>
        {
            new()
            {
                CourseType = courseType,
                IsRestrictedProvider = false
            }
        };

        var standard = new Standard
        {
            LarsCode = "123456",
            Title = "Test Course",
            Level = 2,
            CourseType = CourseType.Apprenticeship,
            RestrictedCourseView = new RestrictedCourseView
            {
                LarsCode = "123456"
            }
        };

        var providerAllowedCourse = new ProviderAllowedCourse
        {
            LarsCode = standard.LarsCode,
            Ukprn = ukprn,
            LastDateStarts = DateTime.UtcNow.Date
        };

        var providerCourse = new List<Domain.Entities.ProviderCourse>
        {
            new()
            {
                Standard = standard,
                ProviderAllowedCourse = providerAllowedCourse
            }
        };

        providerCourseTypesReadRepository
            .Setup(x => x.GetProviderCourseTypesByUkprn(ukprn, cancellationToken))
            .ReturnsAsync(providerCourseType);

        standardsReadRepository
            .Setup(x => x.GetAllStandards())
            .ReturnsAsync(new List<Standard> { standard });

        providerCoursesReadRepository
            .Setup(x => x.GetAllProviderCourses(ukprn))
            .ReturnsAsync(providerCourse);

        // Act
        var response = await sut.Handle(new GetProviderAllowedCoursesQuery(ukprn, CourseType.Apprenticeship), cancellationToken);

        // Assert
        response.AllowedCourses.First().LarsCode.Should().Be(standard.LarsCode);
        response.AllowedCourses.First().Title.Should().Be(standard.Title);
        response.AllowedCourses.First().Level.Should().Be(standard.Level);
        response.AllowedCourses.First().LastDateStarts.Should().Be(providerAllowedCourse.LastDateStarts);
        response.AllowedCourses.First().IsActive.Should().BeTrue();
    }

    [Test, MoqAutoData]
    public async Task WhenCourseIsRestrictedAndProviderAllowedCourseDoesNotExist_ThenDoesNotReturnCourse(
        [Frozen] Mock<IProviderCourseTypesRepository> providerCourseTypesReadRepository,
        [Frozen] Mock<IStandardsReadRepository> standardsReadRepository,
        [Frozen] Mock<IProviderCoursesReadRepository> providerCoursesReadRepository,
        GetProviderAllowedCoursesQueryHandler sut,
        int ukprn,
        CancellationToken cancellationToken)
    {
        // Arrange
        var courseType = CourseType.Apprenticeship;

        var providerCourseType = new List<ProviderCourseType>
        {
            new()
            {
                CourseType = courseType,
                IsRestrictedProvider = false
            }
        };

        var standard = new Standard
        {
            LarsCode = "123456",
            Title = "Test Course",
            Level = 2,
            CourseType = CourseType.Apprenticeship,
            RestrictedCourseView = new RestrictedCourseView
            {
                LarsCode = "123456"
            }
        };

        var providerCourse = new List<Domain.Entities.ProviderCourse>
        {
            new()
            {
                Standard = standard,
                ProviderAllowedCourse = null
            }
        };

        providerCourseTypesReadRepository
            .Setup(x => x.GetProviderCourseTypesByUkprn(ukprn, cancellationToken))
            .ReturnsAsync(providerCourseType);

        standardsReadRepository
            .Setup(x => x.GetAllStandards())
            .ReturnsAsync(new List<Standard> { standard });

        providerCoursesReadRepository
            .Setup(x => x.GetAllProviderCourses(ukprn))
            .ReturnsAsync(providerCourse);

        // Act
        var response = await sut.Handle(
            new GetProviderAllowedCoursesQuery(ukprn, CourseType.Apprenticeship),
            cancellationToken);

        // Assert
        response.AllowedCourses.Should().BeEmpty();
    }
}
