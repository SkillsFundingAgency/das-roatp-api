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
    public async Task WhenProviderCourseTypesIsNull_ThenReturnsEmpty(
        [Frozen] Mock<IProviderCourseTypesRepository> providerCourseTypesReadRepository,
        GetProviderAllowedCoursesQueryHandler sut,
        int ukprn,
        CancellationToken cancellationToken)
    {
        // Arrange
        providerCourseTypesReadRepository
            .Setup(x => x.GetProviderCourseTypesByUkprn(ukprn, cancellationToken))
            .ReturnsAsync((List<ProviderCourseType>)null);

        // Act
        var response = await sut.Handle(
            new GetProviderAllowedCoursesQuery(ukprn, CourseType.Apprenticeship),
            cancellationToken);

        // Assert
        response.AllowedCourses.Should().BeEmpty();
    }

    [Test, MoqAutoData]
    public async Task WhenProviderCourseTypeDoesNotExist_ThenReturnsEmpty(
        [Frozen] Mock<IProviderCourseTypesRepository> providerCourseTypesReadRepository,
        GetProviderAllowedCoursesQueryHandler sut,
        int ukprn,
        CancellationToken cancellationToken)
    {
        // Arrange
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
        var response = await sut.Handle(
            new GetProviderAllowedCoursesQuery(ukprn, CourseType.ShortCourse),
            cancellationToken);

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

        var providerCourseTypes = new List<ProviderCourseType>
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
            .ReturnsAsync(providerCourseTypes);

        providerAllowedCoursesRepository
            .Setup(x => x.GetProviderAllowedCourses(
                ukprn,
                courseType,
                cancellationToken))
            .ReturnsAsync(allowedCourses);

        // Act
        var response = await sut.Handle(
            new GetProviderAllowedCoursesQuery(ukprn, courseType),
            cancellationToken);

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

        var providerCourseTypes = new List<ProviderCourseType>
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
            .ReturnsAsync(providerCourseTypes);

        standardsReadRepository
            .Setup(x => x.GetAllStandards())
            .ReturnsAsync(new List<Standard> { standard });

        providerCoursesReadRepository
            .Setup(x => x.GetAllProviderCourses(ukprn))
            .ReturnsAsync(new List<Domain.Entities.ProviderCourse>());

        // Act
        var response = await sut.Handle(
            new GetProviderAllowedCoursesQuery(ukprn, courseType),
            cancellationToken);

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

        var providerCourseTypes = new List<ProviderCourseType>
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

        var providerCourses = new List<Domain.Entities.ProviderCourse>
        {
            new()
            {
                Standard = standard,
                ProviderAllowedCourse = providerAllowedCourse
            }
        };

        providerCourseTypesReadRepository
            .Setup(x => x.GetProviderCourseTypesByUkprn(ukprn, cancellationToken))
            .ReturnsAsync(providerCourseTypes);

        standardsReadRepository
            .Setup(x => x.GetAllStandards())
            .ReturnsAsync(new List<Standard> { standard });

        providerCoursesReadRepository
            .Setup(x => x.GetAllProviderCourses(ukprn))
            .ReturnsAsync(providerCourses);

        // Act
        var response = await sut.Handle(
            new GetProviderAllowedCoursesQuery(ukprn, courseType),
            cancellationToken);

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

        var providerCourseTypes = new List<ProviderCourseType>
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
            RestrictedCourseView = new RestrictedCourseView
            {
                LarsCode = "123456"
            }
        };

        var providerCourses = new List<Domain.Entities.ProviderCourse>
        {
            new()
            {
                Standard = standard,
                ProviderAllowedCourse = null
            }
        };

        providerCourseTypesReadRepository
            .Setup(x => x.GetProviderCourseTypesByUkprn(ukprn, cancellationToken))
            .ReturnsAsync(providerCourseTypes);

        standardsReadRepository
            .Setup(x => x.GetAllStandards())
            .ReturnsAsync(new List<Standard> { standard });

        providerCoursesReadRepository
            .Setup(x => x.GetAllProviderCourses(ukprn))
            .ReturnsAsync(providerCourses);

        // Act
        var response = await sut.Handle(
            new GetProviderAllowedCoursesQuery(ukprn, courseType),
            cancellationToken);

        // Assert
        response.AllowedCourses.Should().BeEmpty();
    }

    [Test, MoqAutoData]
    public async Task WhenRequestedCourseTypeIsNullAndCourseTypesAreNotRestricted_ThenReturnsCoursesForAllCourseTypes(
        [Frozen] Mock<IProviderCourseTypesRepository> providerCourseTypesReadRepository,
        [Frozen] Mock<IStandardsReadRepository> standardsReadRepository,
        [Frozen] Mock<IProviderCoursesReadRepository> providerCoursesReadRepository,
        GetProviderAllowedCoursesQueryHandler sut,
        int ukprn,
        CancellationToken cancellationToken)
    {
        // Arrange
        var providerCourseTypes = new List<ProviderCourseType>
        {
            new()
            {
                CourseType = CourseType.Apprenticeship,
                IsRestrictedProvider = false
            },
            new()
            {
                CourseType = CourseType.ShortCourse,
                IsRestrictedProvider = false
            }
        };

        var apprenticeshipStandard = new Standard
        {
            LarsCode = "APP001",
            Title = "Apprenticeship Course",
            Level = 3,
            CourseType = CourseType.Apprenticeship,
            RestrictedCourseView = null
        };

        var shortCourseStandard = new Standard
        {
            LarsCode = "SC001",
            Title = "Short Course",
            Level = 2,
            CourseType = CourseType.ShortCourse,
            RestrictedCourseView = null
        };

        var standards = new List<Standard>
        {
            apprenticeshipStandard,
            shortCourseStandard
        };

        providerCourseTypesReadRepository
            .Setup(x => x.GetProviderCourseTypesByUkprn(ukprn, cancellationToken))
            .ReturnsAsync(providerCourseTypes);

        standardsReadRepository
            .Setup(x => x.GetAllStandards())
            .ReturnsAsync(standards);

        providerCoursesReadRepository
            .Setup(x => x.GetAllProviderCourses(ukprn))
            .ReturnsAsync(new List<Domain.Entities.ProviderCourse>());

        // Act
        var response = await sut.Handle(
            new GetProviderAllowedCoursesQuery(ukprn, null),
            cancellationToken);

        // Assert
        response.AllowedCourses.Should().ContainSingle(x => x.LarsCode == apprenticeshipStandard.LarsCode);
        response.AllowedCourses.Should().ContainSingle(x => x.LarsCode == shortCourseStandard.LarsCode);

    }

    [Test, MoqAutoData]
    public async Task WhenRequestedCourseTypeIsNullAndCourseTypesAreRestricted_ThenReturnsAllowedCoursesForAllCourseTypes(
        [Frozen] Mock<IProviderCourseTypesRepository> providerCourseTypesReadRepository,
        [Frozen] Mock<IProviderAllowedCoursesRepository> providerAllowedCoursesRepository,
        GetProviderAllowedCoursesQueryHandler sut,
        int ukprn,
        CancellationToken cancellationToken)
    {
        // Arrange
        var providerCourseTypes = new List<ProviderCourseType>
        {
            new()
            {
                CourseType = CourseType.Apprenticeship,
                IsRestrictedProvider = true
            },
            new()
            {
                CourseType = CourseType.ShortCourse,
                IsRestrictedProvider = true
            }
        };

        var apprenticeshipCourses = new List<ProviderAllowedCourse>
        {
            new()
            {
                LarsCode = "APP001",
                Ukprn = ukprn,
                Standard = new Standard
                {
                    LarsCode = "APP001",
                    Title = "Apprenticeship Course",
                    Level = 3,
                    CourseType = CourseType.Apprenticeship
                }
            }
        };

        var shortCourses = new List<ProviderAllowedCourse>
        {
            new()
            {
                LarsCode = "SC001",
                Ukprn = ukprn,
                Standard = new Standard
                {
                    LarsCode = "SC001",
                    Title = "Short Course",
                    Level = 2,
                    CourseType = CourseType.ShortCourse
                }
            }
        };

        providerCourseTypesReadRepository
            .Setup(x => x.GetProviderCourseTypesByUkprn(ukprn, cancellationToken))
            .ReturnsAsync(providerCourseTypes);

        providerAllowedCoursesRepository
            .Setup(x => x.GetProviderAllowedCourses(
                ukprn,
                CourseType.Apprenticeship,
                cancellationToken))
            .ReturnsAsync(apprenticeshipCourses);

        providerAllowedCoursesRepository
            .Setup(x => x.GetProviderAllowedCourses(
                ukprn,
                CourseType.ShortCourse,
                cancellationToken))
            .ReturnsAsync(shortCourses);

        // Act
        var response = await sut.Handle(
            new GetProviderAllowedCoursesQuery(ukprn, null),
            cancellationToken);

        // Assert
        response.AllowedCourses.Should().ContainSingle(x => x.LarsCode == apprenticeshipCourses[0].LarsCode);
        response.AllowedCourses.Should().ContainSingle(x => x.LarsCode == shortCourses[0].LarsCode);
    }

    [Test, MoqAutoData]
    public async Task WhenRequestedCourseTypeIsNullAndCourseTypesAreRestrictedAndUnrestricted_ThenReturnsCoursesFromAllowedCoursesAndStandards(
        [Frozen] Mock<IProviderCourseTypesRepository> providerCourseTypesReadRepository,
        [Frozen] Mock<IProviderAllowedCoursesRepository> providerAllowedCoursesRepository,
        [Frozen] Mock<IStandardsReadRepository> standardsReadRepository,
        [Frozen] Mock<IProviderCoursesReadRepository> providerCoursesReadRepository,
        GetProviderAllowedCoursesQueryHandler sut,
        int ukprn,
        CancellationToken cancellationToken)
    {
        // Arrange
        var providerCourseTypes = new List<ProviderCourseType>
        {
            new()
            {
                CourseType = CourseType.Apprenticeship,
                IsRestrictedProvider = true
            },
            new()
            {
                CourseType = CourseType.ShortCourse,
                IsRestrictedProvider = false
            }
        };

        var restrictedCourse = new ProviderAllowedCourse
        {
            LarsCode = "APP001",
            Ukprn = ukprn,
            Standard = new Standard
            {
                LarsCode = "APP001",
                Title = "Restricted Apprenticeship",
                Level = 3,
                CourseType = CourseType.Apprenticeship
            }
        };

        var shortCourseStandard = new Standard
        {
            LarsCode = "SC001",
            Title = "Available Short Course",
            Level = 2,
            CourseType = CourseType.ShortCourse,
            RestrictedCourseView = null
        };

        providerCourseTypesReadRepository
            .Setup(x => x.GetProviderCourseTypesByUkprn(ukprn, cancellationToken))
            .ReturnsAsync(providerCourseTypes);

        providerAllowedCoursesRepository
            .Setup(x => x.GetProviderAllowedCourses(
                ukprn,
                CourseType.Apprenticeship,
                cancellationToken))
            .ReturnsAsync(new List<ProviderAllowedCourse> { restrictedCourse });

        standardsReadRepository
            .Setup(x => x.GetAllStandards())
            .ReturnsAsync(new List<Standard> { shortCourseStandard });

        providerCoursesReadRepository
            .Setup(x => x.GetAllProviderCourses(ukprn))
            .ReturnsAsync(new List<Domain.Entities.ProviderCourse>());

        // Act
        var response = await sut.Handle(
            new GetProviderAllowedCoursesQuery(ukprn, null),
            cancellationToken);

        // Assert
        response.AllowedCourses.Should().ContainSingle(x => x.LarsCode == shortCourseStandard.LarsCode);
        response.AllowedCourses.Should().ContainSingle(x => x.LarsCode == restrictedCourse.LarsCode);
    }
}