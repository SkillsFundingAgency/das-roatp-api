using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit4;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.ProviderCoursesTimelines.Queries.GetAllProviderCoursesTimelines;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.Application.UnitTests.ProviderCoursesTimelines.Queries.GetAllProviderCoursesTimelines;

public class GetAllProviderCoursesTimelinesQueryHandlerTests
{
    [Test, RecursiveMoqAutoData]
    public async Task Handle_ReturnsExpectedResults(
        [Frozen] Mock<IProviderCoursesTimelineRepository> repoMock,
        GetAllProviderCoursesTimelinesQueryHandler sut,
        GetAllProviderCoursesTimelinesQuery request,
        List<ProviderTimelineExport> providers,
        CancellationToken cancellationToken)
    {
        GetAllProviderCoursesTimelinesQueryResult expected = providers;

        repoMock
            .Setup(x => x.GetProviderTimelineExport(null, cancellationToken))
            .ReturnsAsync(providers);

        GetAllProviderCoursesTimelinesQueryResult actualResult = await sut.Handle(null, cancellationToken);

        actualResult.Should().BeEquivalentTo(expected);

        repoMock.Verify(
            x => x.GetProviderTimelineExport(null, cancellationToken),
            Times.Once);
    }
}
