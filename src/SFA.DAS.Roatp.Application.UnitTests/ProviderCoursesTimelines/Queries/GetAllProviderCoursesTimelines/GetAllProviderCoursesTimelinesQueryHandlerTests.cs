using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit4;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.ProviderCoursesTimelines.Queries.GetAllProviderCoursesTimelines;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Roatp.Domain.Models;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.Application.UnitTests.ProviderCoursesTimelines.Queries.GetAllProviderCoursesTimelines;

public class GetAllProviderCoursesTimelinesQueryHandlerTests
{
    [Test, RecursiveMoqAutoData]
    public async Task Handle_ReturnsExpectedResults(
        [Frozen] Mock<IProviderCoursesTimelineRepository> repoMock,
        GetAllProviderCoursesTimelinesQueryHandler sut,
        GetAllProviderCoursesTimelinesQuery request,
        ProviderRegistrationDetail providerRegistrationDetail,
        CancellationToken cancellationToken)
    {
        // Arrange
        var larsCode = "LARS123";
        providerRegistrationDetail.StatusId = 1;
        providerRegistrationDetail.ProviderTypeId = 1;
        providerRegistrationDetail.ProviderCourseTypes = new List<ProviderCourseType>()
        {
            new ProviderCourseType
                {
                    CourseType = CourseType.Apprenticeship,
                },
                new ProviderCourseType
                {
                    CourseType = CourseType.ShortCourse
                }
        };
        providerRegistrationDetail.Provider.ProviderCoursesTimelines = new List<ProviderCoursesTimeline>()
        {
            new ProviderCoursesTimeline
                    {
                        LarsCode = larsCode,
                        EffectiveFrom = new DateTime(2023, 1, 1, 0,0,0, DateTimeKind.Unspecified),
                        EffectiveTo = null,
                        Standard = new Standard { CourseType = CourseType.Apprenticeship }
                    }
        };
        providerRegistrationDetail.Provider.ProviderAllowedCourses = new List<ProviderAllowedCourse>()
        {
            new ProviderAllowedCourse
                {
                    LarsCode = larsCode,
                    Ukprn = 12345678,
                    LastDateStarts = DateTime.Today
                }
        };
        providerRegistrationDetail.Provider.Courses = new List<Domain.Entities.ProviderCourse>()
        {
            new Domain.Entities.ProviderCourse
                {
                    LarsCode = larsCode,
                    ProviderId = providerRegistrationDetail.Provider.Id,
                }
        };
        List<ProviderRegistrationDetail> providersData = [providerRegistrationDetail];

        GetAllProviderCoursesTimelinesQueryResult expected = providersData;
        repoMock.Setup(x => x.GetAllProviderCoursesTimelines(cancellationToken)).ReturnsAsync(providersData);
        // Act
        GetAllProviderCoursesTimelinesQueryResult actualResult = await sut.Handle(request, cancellationToken);
        // Assert
        actualResult.Should().BeEquivalentTo(expected);
    }

    [Test, RecursiveMoqAutoData]
    public async Task Handle_CourseIsNotAllowed_ReturnsLastDateStartsAsNull(
    [Frozen] Mock<IProviderCoursesTimelineRepository> repoMock,
    GetAllProviderCoursesTimelinesQueryHandler sut,
    GetAllProviderCoursesTimelinesQuery request,
    ProviderRegistrationDetail providerRegistrationDetail,
    CancellationToken cancellationToken)
    {
        // Arrange
        var larsCode = "LARS123";

        providerRegistrationDetail.StatusId = 1;
        providerRegistrationDetail.ProviderTypeId = 1;

        providerRegistrationDetail.ProviderCourseTypes = new List<ProviderCourseType>
            {
                new ProviderCourseType
                {
                    CourseType = CourseType.Apprenticeship,
                    Ukprn = providerRegistrationDetail.Ukprn
                }
            };

        providerRegistrationDetail.Provider.ProviderCoursesTimelines = new List<ProviderCoursesTimeline>
            {
                new ProviderCoursesTimeline
                {
                    LarsCode = larsCode,
                    EffectiveFrom = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Unspecified),
                    EffectiveTo = null,
                    Standard = new Standard { CourseType = CourseType.Apprenticeship }
                }
            };

        providerRegistrationDetail.Provider.ProviderAllowedCourses = [];

        providerRegistrationDetail.Provider.Courses = new List<Domain.Entities.ProviderCourse>
            {
                new Domain.Entities.ProviderCourse
                {
                    LarsCode = larsCode,
                    ProviderId = providerRegistrationDetail.Provider.Id
                }
            };

        List<ProviderRegistrationDetail> providersData = [providerRegistrationDetail];

        repoMock.Setup(x => x.GetAllProviderCoursesTimelines(cancellationToken)).ReturnsAsync(providersData);

        // Act
        GetAllProviderCoursesTimelinesQueryResult actualResult = await sut.Handle(request, cancellationToken);

        // Assert
        actualResult.Providers.First().CourseTypes.First().Courses.First().LastDateStarts.Should().BeNull();
    }

    [Test, RecursiveMoqAutoData]
    public async Task Handle_ProviderDoesNotProvideCourse_ReturnsLastDateStartsAsNull(
        [Frozen] Mock<IProviderCoursesTimelineRepository> repoMock,
        GetAllProviderCoursesTimelinesQueryHandler sut,
        GetAllProviderCoursesTimelinesQuery request,
        ProviderRegistrationDetail providerRegistrationDetail,
        CancellationToken cancellationToken)
    {
        // Arrange
        var larsCode = "LARS123";

        providerRegistrationDetail.StatusId = 1;
        providerRegistrationDetail.ProviderTypeId = 1;

        providerRegistrationDetail.ProviderCourseTypes = new List<ProviderCourseType>
            {
                new ProviderCourseType
                {
                    CourseType = CourseType.Apprenticeship,
                    Ukprn = providerRegistrationDetail.Ukprn
                }
            };

        providerRegistrationDetail.Provider.ProviderCoursesTimelines = new List<ProviderCoursesTimeline>
            {
                new ProviderCoursesTimeline
                {
                    LarsCode = larsCode,
                    EffectiveFrom = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Unspecified),
                    EffectiveTo = null,
                    Standard = new Standard { CourseType = CourseType.Apprenticeship }
                }
            };

        providerRegistrationDetail.Provider.ProviderAllowedCourses = new List<ProviderAllowedCourse>()
        {
            new ProviderAllowedCourse
                {
                    LarsCode = larsCode,
                    Ukprn = 12345678,
                    LastDateStarts = DateTime.Today
                }
        };

        providerRegistrationDetail.Provider.Courses = [];

        List<ProviderRegistrationDetail> providersData = [providerRegistrationDetail];

        repoMock.Setup(x => x.GetAllProviderCoursesTimelines(cancellationToken)).ReturnsAsync(providersData);

        // Act
        GetAllProviderCoursesTimelinesQueryResult actualResult = await sut.Handle(request, cancellationToken);
        // Assert
        actualResult.Providers.FirstOrDefault().CourseTypes.FirstOrDefault().Courses.FirstOrDefault().LastDateStarts.Should().BeNull();
    }
}
