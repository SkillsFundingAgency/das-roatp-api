using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
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
        ProviderRegistrationDetail providerRegistrationDetail,
        CancellationToken cancellationToken)
    {
        // Arrange
        providerRegistrationDetail.StatusId = 1;
        providerRegistrationDetail.ProviderTypeId = 1;
        providerRegistrationDetail.Provider.ProviderCourseTypes = [];
        providerRegistrationDetail.Provider.ProviderCoursesTimelines = [];
        List<ProviderRegistrationDetail> providersData = [providerRegistrationDetail];

        GetAllProviderCoursesTimelinesQueryResult expected = providersData;
        repoMock.Setup(x => x.GetAllProviderCoursesTimelines(cancellationToken)).ReturnsAsync(providersData);
        // Act
        GetAllProviderCoursesTimelinesQueryResult actualResult = await sut.Handle(request, cancellationToken);
        // Assert
        actualResult.Should().BeEquivalentTo(expected);
    }
}
