using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit4;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.ProviderCoursesTimelines.Queries;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Roatp.Domain.Models;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.Application.UnitTests.ProviderCoursesTimelines.Queries.GetProviderCoursesTimelines;

public class GetProviderCoursesTimelinesQueryHandlerTests
{
    [Test, MoqAutoData]
    public async Task Handle_ReturnsCorrectResult(
        [Frozen] Mock<IProviderCoursesTimelineRepository> repoMock,
        GetProviderCoursesTimelinesQuery query,
        GetProviderCoursesTimelinesQueryHandler sut,
        CancellationToken cancellationToken)
    {
        var providers = new List<ProviderTimelineExport>
        {
            new()
            {
                Ukprn = query.Ukprn,
                StatusId = (int)ProviderStatusType.Active,
                ProviderTypeId = (int)ProviderType.Main,
                CourseType = CourseType.Apprenticeship,
                LarsCode = "100",
                EffectiveFrom = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                EffectiveTo = new DateTime(2024, 6, 1, 0, 0, 0, DateTimeKind.Utc),
                LastDateStarts = new DateTime(2024, 5, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new()
            {
                Ukprn = query.Ukprn,
                StatusId = (int)ProviderStatusType.Active,
                ProviderTypeId = (int)ProviderType.Main,
                CourseType = CourseType.ShortCourse,
                LarsCode = null,
                EffectiveFrom = null,
                EffectiveTo = null,
                LastDateStarts = null
            }
        };

        repoMock
            .Setup(r => r.GetProviderTimelineExport(query.Ukprn, cancellationToken))
            .ReturnsAsync(providers);

        ProviderCoursesTimelineModel expected = providers;

        ProviderCoursesTimelineModel actual = await sut.Handle(query, cancellationToken);

        actual.Should().BeEquivalentTo(expected);

        repoMock.Verify(
            r => r.GetProviderTimelineExport(query.Ukprn, cancellationToken),
            Times.Once);
    }

    [Test, MoqAutoData]
    public async Task Handle_ReturnsNullResult(
        [Frozen] Mock<IProviderCoursesTimelineRepository> repoMock,
        GetProviderCoursesTimelinesQuery query,
        GetProviderCoursesTimelinesQueryHandler sut,
        CancellationToken cancellationToken)
    {
        repoMock
            .Setup(r => r.GetProviderTimelineExport(query.Ukprn, cancellationToken))
            .ReturnsAsync((List<ProviderTimelineExport>)null);

        ProviderCoursesTimelineModel actual = await sut.Handle(query, cancellationToken);

        actual.Should().BeNull();

        repoMock.Verify(
            r => r.GetProviderTimelineExport(query.Ukprn, cancellationToken),
            Times.Once);
    }
}
