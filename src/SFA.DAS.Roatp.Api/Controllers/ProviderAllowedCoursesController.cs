using System.Threading;
using System.Threading.Tasks;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Roatp.Application.ProviderAllowedCourses.Queries.GetProviderAllowedCourses;
using SFA.DAS.Roatp.Domain.Models;
using static SFA.DAS.Roatp.Api.Infrastructure.Constants;

namespace SFA.DAS.Roatp.Api.Controllers;

[ApiController]
[ApiVersion(ApiVersionNumber.One)]
[Tags(EndpointTags.ProviderAllowedCourses)]
[Route("providers/{ukprn}/allowed-courses")]
public class ProviderAllowedCoursesController(IMediator _mediator) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(GetProviderAllowedCoursesQueryResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllowedCourses([FromRoute] int ukprn, [FromQuery] CourseType courseType = CourseType.ShortCourse, CancellationToken cancellationToken = default)
    {
        GetProviderAllowedCoursesQuery query = new(ukprn, courseType);
        GetProviderAllowedCoursesQueryResult result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }
}
