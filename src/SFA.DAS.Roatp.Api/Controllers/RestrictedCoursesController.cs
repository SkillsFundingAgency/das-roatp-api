using System.Threading.Tasks;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Roatp.Api.Infrastructure;
using SFA.DAS.Roatp.Application.RestrictedCourses.Queries.GetAllRestrictedCourses;
using static SFA.DAS.Roatp.Api.Infrastructure.Constants;

namespace SFA.DAS.Roatp.Api.Controllers;

[ApiController]
[ApiVersion(ApiVersionNumber.One)]
[Route("/restricted-courses")]
public class RestrictedCoursesController(IMediator _mediator) : ActionResponseControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(GetAllRestrictedCoursesQueryResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllRestrictedCourses([FromQuery] bool restricted)
    {
        GetAllRestrictedCoursesQuery query = new(restricted);
        GetAllRestrictedCoursesQueryResult result = await _mediator.Send(query);
        return Ok(result);
    }
}
