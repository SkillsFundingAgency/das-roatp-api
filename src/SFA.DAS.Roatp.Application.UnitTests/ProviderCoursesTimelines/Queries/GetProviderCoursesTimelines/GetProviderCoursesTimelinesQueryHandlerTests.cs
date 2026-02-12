using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.ProviderCoursesTimelines.Queries;
using SFA.DAS.Roatp.Domain.Interfaces;
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
        repoMock.Setup(r => r.GetProviderCoursesTimelines(query.Ukprn, cancellationToken)).ReturnsAsync(TestDataHelper.GetProviderRegistrationDetails());

        ProviderCoursesTimelineModel expected = TestDataHelper.GetProviderRegistrationDetails();

        ProviderCoursesTimelineModel actual = await sut.Handle(query, cancellationToken);

        actual.Should().BeEquivalentTo(expected);
    }

    [Test, MoqAutoData]
    public async Task Handle_ReturnsNullResult(
        [Frozen] Mock<IProviderCoursesTimelineRepository> repoMock,
        GetProviderCoursesTimelinesQuery query,
        GetProviderCoursesTimelinesQueryHandler sut,
        CancellationToken cancellationToken)
    {
        repoMock.Setup(r => r.GetProviderCoursesTimelines(query.Ukprn, cancellationToken)).ReturnsAsync(() => null);

        ProviderCoursesTimelineModel actual = await sut.Handle(query, cancellationToken);

        actual.Should().BeNull();
    }
}
