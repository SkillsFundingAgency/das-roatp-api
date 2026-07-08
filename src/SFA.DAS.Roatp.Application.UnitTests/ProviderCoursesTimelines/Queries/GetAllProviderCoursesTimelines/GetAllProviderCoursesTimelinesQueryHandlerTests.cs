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
        providerRegistrationDetail.StatusId = 1;
        providerRegistrationDetail.ProviderTypeId = 1;
        providerRegistrationDetail.Provider.ProviderCourseTypes = new List<ProviderCourseType>()
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
                        LarsCode = "LARS123",
                        EffectiveFrom = new DateTime(2023, 1, 1, 0,0,0, DateTimeKind.Unspecified),
                        EffectiveTo = null,
                        Standard = new Standard { CourseType = CourseType.Apprenticeship }
                    }
        };
        providerRegistrationDetail.ProviderAllowedCourses = new List<ProviderAllowedCourse>()
        {
            new ProviderAllowedCourse
                {
                    LarsCode = "LARS123",
                    Ukprn = 12345678,
                    LastDateStarts = DateTime.Today
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
    public async Task Handle_LastDateStartsNotAvailable_ReturnsLastDateStartsAsNull(
        [Frozen] Mock<IProviderCoursesTimelineRepository> repoMock,
        GetAllProviderCoursesTimelinesQueryHandler sut,
        GetAllProviderCoursesTimelinesQuery request,
        ProviderRegistrationDetail providerRegistrationDetail,
        CancellationToken cancellationToken)
    {
        // Arrange
        providerRegistrationDetail.StatusId = 1;
        providerRegistrationDetail.ProviderTypeId = 1;
        providerRegistrationDetail.Provider.ProviderCourseTypes = new List<ProviderCourseType>()
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
                        LarsCode = "LARS123",
                        EffectiveFrom = new DateTime(2023, 1, 1, 0,0,0, DateTimeKind.Unspecified),
                        EffectiveTo = null,
                        Standard = new Standard { CourseType = CourseType.Apprenticeship }
                    }
        };
        List<ProviderRegistrationDetail> providersData = [providerRegistrationDetail];

        GetAllProviderCoursesTimelinesQueryResult expected = providersData;
        repoMock.Setup(x => x.GetAllProviderCoursesTimelines(cancellationToken)).ReturnsAsync(providersData);
        // Act
        GetAllProviderCoursesTimelinesQueryResult actualResult = await sut.Handle(request, cancellationToken);
        // Assert
        actualResult.Providers.FirstOrDefault().CourseTypes.FirstOrDefault().Courses.FirstOrDefault().LastDateStarts.Should().BeNull();
    }
}
