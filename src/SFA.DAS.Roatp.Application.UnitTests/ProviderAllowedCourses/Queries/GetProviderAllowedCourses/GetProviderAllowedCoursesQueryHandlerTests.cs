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
using SFA.DAS.Roatp.Domain.Constants;
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
        [Frozen] Mock<IProviderAllowedCoursesRepository> providerAllowedCoursesRepository,
        [Frozen] Mock<IStandardsReadRepository> standardsReadRepository,
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

        providerAllowedCoursesRepository
            .Setup(x => x.GetProviderAllowedCourses(
                ukprn,
                courseType,
                cancellationToken))
            .ReturnsAsync(new List<ProviderAllowedCourse>());

        // Act
        var response = await sut.Handle(
            new GetProviderAllowedCoursesQuery(ukprn, courseType),
            cancellationToken);

        // Assert
        response.AllowedCourses.First().LarsCode.Should().Be(standard.LarsCode);
        response.AllowedCourses.First().Title.Should().Be(standard.Title);
        response.AllowedCourses.First().Level.Should().Be(standard.Level);
    }

    [Test, MoqAutoData]
    public async Task WhenCourseIsRestrictedAndProviderAllowedCourseExists_ThenReturnsCourse(
        [Frozen] Mock<IProviderCourseTypesRepository> providerCourseTypesReadRepository,
        [Frozen] Mock<IProviderAllowedCoursesRepository> providerAllowedCoursesRepository,
        [Frozen] Mock<IStandardsReadRepository> standardsReadRepository,
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

        providerCourseTypesReadRepository
            .Setup(x => x.GetProviderCourseTypesByUkprn(ukprn, cancellationToken))
            .ReturnsAsync(providerCourseTypes);

        standardsReadRepository
            .Setup(x => x.GetAllStandards())
            .ReturnsAsync(new List<Standard> { standard });

        providerAllowedCoursesRepository
            .Setup(x => x.GetProviderAllowedCourses(
                ukprn,
                courseType,
                cancellationToken))
            .ReturnsAsync(new List<ProviderAllowedCourse>
            {
                providerAllowedCourse
            });

        // Act
        var response = await sut.Handle(
            new GetProviderAllowedCoursesQuery(ukprn, courseType),
            cancellationToken);

        // Assert
        response.AllowedCourses.First().LarsCode.Should().Be(standard.LarsCode);
        response.AllowedCourses.First().Title.Should().Be(standard.Title);
        response.AllowedCourses.First().Level.Should().Be(standard.Level);
    }


    [Test, MoqAutoData]
    public async Task WhenCourseIsRestrictedAndProviderAllowedCourseDoesNotExist_ThenDoesNotReturnCourse(
        [Frozen] Mock<IProviderCourseTypesRepository> providerCourseTypesReadRepository,
        [Frozen] Mock<IProviderAllowedCoursesRepository> providerAllowedCoursesRepository,
        [Frozen] Mock<IStandardsReadRepository> standardsReadRepository,
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

        providerCourseTypesReadRepository
            .Setup(x => x.GetProviderCourseTypesByUkprn(ukprn, cancellationToken))
            .ReturnsAsync(providerCourseTypes);

        standardsReadRepository
            .Setup(x => x.GetAllStandards())
            .ReturnsAsync(new List<Standard> { standard });

        providerAllowedCoursesRepository
            .Setup(x => x.GetProviderAllowedCourses(
                ukprn,
                courseType,
                cancellationToken))
            .ReturnsAsync(new List<ProviderAllowedCourse>());

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
        [Frozen] Mock<IProviderAllowedCoursesRepository> providerAllowedCoursesRepository,
        [Frozen] Mock<IStandardsReadRepository> standardsReadRepository,
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

        providerCourseTypesReadRepository
            .Setup(x => x.GetProviderCourseTypesByUkprn(ukprn, cancellationToken))
            .ReturnsAsync(providerCourseTypes);

        standardsReadRepository
            .Setup(x => x.GetAllStandards())
            .ReturnsAsync(new List<Standard>
            {
                apprenticeshipStandard,
                shortCourseStandard
            });

        providerAllowedCoursesRepository
            .Setup(x => x.GetProviderAllowedCourses(
                ukprn,
                It.IsAny<CourseType>(),
                cancellationToken))
            .ReturnsAsync(new List<ProviderAllowedCourse>());

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
            .ReturnsAsync(new List<ProviderAllowedCourse>
            {
                restrictedCourse
            });

        providerAllowedCoursesRepository
            .Setup(x => x.GetProviderAllowedCourses(
                ukprn,
                CourseType.ShortCourse,
                cancellationToken))
            .ReturnsAsync(new List<ProviderAllowedCourse>());

        standardsReadRepository
            .Setup(x => x.GetAllStandards())
            .ReturnsAsync(new List<Standard>
            {
                shortCourseStandard
            });

        // Act
        var response = await sut.Handle(
            new GetProviderAllowedCoursesQuery(ukprn, null),
            cancellationToken);

        // Assert
        response.AllowedCourses.Should().ContainSingle(x => x.LarsCode == shortCourseStandard.LarsCode);

        response.AllowedCourses.Should().ContainSingle(x => x.LarsCode == restrictedCourse.LarsCode);
    }

    [Test, MoqAutoData]
    public async Task WhenProviderAllowedCourseExists_ThenLastDateStartsIsSetFromProviderAllowedCourse(
    [Frozen] Mock<IProviderCourseTypesRepository> providerCourseTypesReadRepository,
    [Frozen] Mock<IProviderAllowedCoursesRepository> providerAllowedCoursesRepository,
    [Frozen] Mock<IStandardsReadRepository> standardsReadRepository,
    GetProviderAllowedCoursesQueryHandler sut,
    int ukprn,
    CancellationToken cancellationToken)
    {
        // Arrange
        var courseType = CourseType.Apprenticeship;
        var lastDateStarts = DateTime.UtcNow.Date.AddDays(10);

        var standard = new Standard
        {
            LarsCode = "123456",
            Title = "Test Course",
            Level = 2,
            CourseType = courseType,
            RestrictedCourseView = null
        };

        var providerAllowedCourse = new ProviderAllowedCourse
        {
            LarsCode = standard.LarsCode,
            Ukprn = ukprn,
            LastDateStarts = lastDateStarts
        };

        providerCourseTypesReadRepository
            .Setup(x => x.GetProviderCourseTypesByUkprn(ukprn, cancellationToken))
            .ReturnsAsync(new List<ProviderCourseType>
            {
            new()
            {
                CourseType = courseType,
                IsRestrictedProvider = false
            }
            });

        standardsReadRepository
            .Setup(x => x.GetAllStandards())
            .ReturnsAsync(new List<Standard> { standard });

        providerAllowedCoursesRepository
            .Setup(x => x.GetProviderAllowedCourses(
                ukprn,
                courseType,
                cancellationToken))
            .ReturnsAsync(new List<ProviderAllowedCourse>
            {
            providerAllowedCourse
            });

        // Act
        var response = await sut.Handle(
            new GetProviderAllowedCoursesQuery(ukprn, courseType),
            cancellationToken);

        // Assert
        response.AllowedCourses.Single().LastDateStarts.Should().Be(lastDateStarts);
    }

    [Test, MoqAutoData]
    public async Task WhenProviderAllowedCourseDoesNotExist_ThenLastDateStartsIsNull(
    [Frozen] Mock<IProviderCourseTypesRepository> providerCourseTypesReadRepository,
    [Frozen] Mock<IProviderAllowedCoursesRepository> providerAllowedCoursesRepository,
    [Frozen] Mock<IStandardsReadRepository> standardsReadRepository,
    GetProviderAllowedCoursesQueryHandler sut,
    int ukprn,
    CancellationToken cancellationToken)
    {
        // Arrange
        var courseType = CourseType.Apprenticeship;

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
            .ReturnsAsync(new List<ProviderCourseType>
            {
            new()
            {
                CourseType = courseType,
                IsRestrictedProvider = false
            }
            });

        standardsReadRepository
            .Setup(x => x.GetAllStandards())
            .ReturnsAsync(new List<Standard> { standard });

        providerAllowedCoursesRepository
            .Setup(x => x.GetProviderAllowedCourses(
                ukprn,
                courseType,
                cancellationToken))
            .ReturnsAsync(new List<ProviderAllowedCourse>());

        // Act
        var response = await sut.Handle(
            new GetProviderAllowedCoursesQuery(ukprn, courseType),
            cancellationToken);

        // Assert
        response.AllowedCourses.Single().LastDateStarts.Should().BeNull();
    }

    [Test, MoqAutoData]
    public async Task WhenLastDateStartsIsStartRestrictedDate_ThenLastDateStartsIsNull(
        [Frozen] Mock<IProviderCourseTypesRepository> providerCourseTypesReadRepository,
        [Frozen] Mock<IProviderAllowedCoursesRepository> providerAllowedCoursesRepository,
        [Frozen] Mock<IStandardsReadRepository> standardsReadRepository,
        GetProviderAllowedCoursesQueryHandler sut,
        int ukprn,
        CancellationToken cancellationToken)
    {
        // Arrange
        var courseType = CourseType.Apprenticeship;

        var standard = new Standard
        {
            LarsCode = "123456",
            Title = "Test Course",
            Level = 2,
            CourseType = courseType,
            RestrictedCourseView = null
        };

        var providerAllowedCourse = new ProviderAllowedCourse
        {
            LarsCode = standard.LarsCode,
            Ukprn = ukprn,
            LastDateStarts = DateConstants.StartRestrictedDate
        };

        providerCourseTypesReadRepository
            .Setup(x => x.GetProviderCourseTypesByUkprn(ukprn, cancellationToken))
            .ReturnsAsync(new List<ProviderCourseType>
            {
                new()
                {
                    CourseType = courseType,
                    IsRestrictedProvider = false
                }
            });

        standardsReadRepository
            .Setup(x => x.GetAllStandards())
            .ReturnsAsync(new List<Standard> { standard });

        providerAllowedCoursesRepository
            .Setup(x => x.GetProviderAllowedCourses(
                ukprn,
                courseType,
                cancellationToken))
            .ReturnsAsync(new List<ProviderAllowedCourse>
            {
                providerAllowedCourse
            });

        // Act
        var response = await sut.Handle(
            new GetProviderAllowedCoursesQuery(ukprn, courseType),
            cancellationToken);

        // Assert
        response.AllowedCourses.Single().LastDateStarts.Should().BeNull();
    }

    [MoqInlineAutoData(-1, true)]
    [MoqInlineAutoData(0, false)]
    [MoqInlineAutoData(1, false)]
    public async Task WhenProviderAllowedCourseHasLastDateStarts_ThenSetsIsClosedToNewStarts(
        int daysFromToday,
        bool expectedIsClosedToNewStarts,
        [Frozen] Mock<IProviderCourseTypesRepository> providerCourseTypesReadRepository,
        [Frozen] Mock<IProviderAllowedCoursesRepository> providerAllowedCoursesRepository,
        [Frozen] Mock<IStandardsReadRepository> standardsReadRepository,
        GetProviderAllowedCoursesQueryHandler sut,
        int ukprn,
        CancellationToken cancellationToken)
    {
        // Arrange
        var courseType = CourseType.Apprenticeship;

        var standard = new Standard
        {
            LarsCode = "123456",
            Title = "Test Course",
            Level = 2,
            CourseType = courseType,
            RestrictedCourseView = null
        };

        var providerAllowedCourse = new ProviderAllowedCourse
        {
            LarsCode = standard.LarsCode,
            Ukprn = ukprn,
            LastDateStarts = DateTime.UtcNow.Date.AddDays(daysFromToday)
        };

        providerCourseTypesReadRepository
            .Setup(x => x.GetProviderCourseTypesByUkprn(ukprn, cancellationToken))
            .ReturnsAsync(new List<ProviderCourseType>
            {
            new()
            {
                CourseType = courseType,
                IsRestrictedProvider = false
            }
            });

        standardsReadRepository
            .Setup(x => x.GetAllStandards())
            .ReturnsAsync(new List<Standard> { standard });

        providerAllowedCoursesRepository
            .Setup(x => x.GetProviderAllowedCourses(
                ukprn,
                courseType,
                cancellationToken))
            .ReturnsAsync(new List<ProviderAllowedCourse>
            {
            providerAllowedCourse
            });

        // Act
        var response = await sut.Handle(
            new GetProviderAllowedCoursesQuery(ukprn, courseType),
            cancellationToken);

        // Assert
        response.AllowedCourses.Single().IsClosedToNewStarts.Should().Be(expectedIsClosedToNewStarts);
    }

    [Test, MoqAutoData]
    public async Task WhenLastDateStartsIsStartRestrictedDate_ThenIsClosedToNewStartsIsTrue(
        [Frozen] Mock<IProviderCourseTypesRepository> providerCourseTypesReadRepository,
        [Frozen] Mock<IProviderAllowedCoursesRepository> providerAllowedCoursesRepository,
        [Frozen] Mock<IStandardsReadRepository> standardsReadRepository,
        GetProviderAllowedCoursesQueryHandler sut,
        int ukprn,
        CancellationToken cancellationToken)
    {
        // Arrange
        var courseType = CourseType.Apprenticeship;

        var standard = new Standard
        {
            LarsCode = "123456",
            Title = "Test Course",
            Level = 2,
            CourseType = courseType,
            RestrictedCourseView = null
        };

        var providerAllowedCourse = new ProviderAllowedCourse
        {
            LarsCode = standard.LarsCode,
            Ukprn = ukprn,
            LastDateStarts = DateConstants.StartRestrictedDate
        };

        providerCourseTypesReadRepository
            .Setup(x => x.GetProviderCourseTypesByUkprn(ukprn, cancellationToken))
            .ReturnsAsync(new List<ProviderCourseType>
            {
                new()
                {
                    CourseType = courseType,
                    IsRestrictedProvider = false
                }
            });

        standardsReadRepository
            .Setup(x => x.GetAllStandards())
            .ReturnsAsync(new List<Standard> { standard });

        providerAllowedCoursesRepository
            .Setup(x => x.GetProviderAllowedCourses(
                ukprn,
                courseType,
                cancellationToken))
            .ReturnsAsync(new List<ProviderAllowedCourse>
            {
                providerAllowedCourse
            });

        // Act
        var response = await sut.Handle(
            new GetProviderAllowedCoursesQuery(ukprn, courseType),
            cancellationToken);

        // Assert
        response.AllowedCourses.Single().IsClosedToNewStarts.Should().BeTrue();
    }
}