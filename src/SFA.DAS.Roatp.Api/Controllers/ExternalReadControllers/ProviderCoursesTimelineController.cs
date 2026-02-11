using System.Threading;
using System.Threading.Tasks;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Roatp.Application.ProviderCoursesTimelines.Queries.GetAllProviderCoursesTimelines;
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
    public async Task<ActionResult> GetAllProviderCoursesTimeline(CancellationToken cancellationToken)
    {
        GetAllProviderCoursesTimelinesQueryResult result = await _mediator.Send(new GetAllProviderCoursesTimelinesQuery(), cancellationToken);
        return Ok(result);
    }
}
