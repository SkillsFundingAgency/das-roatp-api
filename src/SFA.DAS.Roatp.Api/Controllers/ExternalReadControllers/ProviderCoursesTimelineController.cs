using System.Threading;
using System.Threading.Tasks;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Roatp.Application.ProviderCoursesTimelines.Queries;
using SFA.DAS.Roatp.Application.ProviderCoursesTimelines.Queries.GetAllProviderCoursesTimelines;
using SFA.DAS.Roatp.Application.UnitTests.ProviderCoursesTimelines.Queries.GetProviderCoursesTimelines;
using static SFA.DAS.Roatp.Api.Infrastructure.Constants;

namespace SFA.DAS.Roatp.Api.Controllers.ExternalReadControllers;

[Route("api/provider-courses-timeline")]
[ApiVersion(ApiVersionNumber.Two)]
[ApiController]
[Tags(EndpointTags.ProviderCourses)]
public class ProviderCoursesTimelineController(IMediator _mediator) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<GetAllProviderCoursesTimelinesQueryResult>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllProviderCoursesTimeline(CancellationToken cancellationToken)
    {
        GetAllProviderCoursesTimelinesQueryResult result = await _mediator.Send(new GetAllProviderCoursesTimelinesQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpGet]
    [Route("{ukprn}")]
    [ProducesResponseType<ProviderCoursesTimelineModel>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProviderCoursesTimeline([FromRoute] int ukprn, CancellationToken cancellationToken)
    {
        ProviderCoursesTimelineModel result = await _mediator.Send(new GetProviderCoursesTimelinesQuery(ukprn), cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }
}
