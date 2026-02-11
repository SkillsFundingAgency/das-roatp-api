using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Api.Controllers.ExternalReadControllers;
using SFA.DAS.Roatp.Application.ProviderCoursesTimelines.Queries.GetAllProviderCoursesTimelines;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.Api.UnitTests.Controllers.ExternalReadControllers;

public class ProviderCoursesTimelineControllerTests
{
    [Test, MoqAutoData]
    public async Task GetAllProviderCoursesTimeline_ReturnsOkResult(
        [Frozen] Mock<IMediator> mediatorMock,
        [Greedy] ProviderCoursesTimelineController controller,
        GetAllProviderCoursesTimelinesQueryResult expected,
        CancellationToken cancellationToken)
    {
        mediatorMock.Setup(m => m.Send(It.IsAny<GetAllProviderCoursesTimelinesQuery>(), cancellationToken)).ReturnsAsync(expected);

        var result = await controller.GetAllProviderCoursesTimeline(cancellationToken);

        result.As<OkObjectResult>().Value.Should().Be(expected);
    }
}
