using System.Threading.Tasks;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Api.Infrastructure;
using SFA.DAS.Roatp.Application.RestrictedCourses.Commands.AddRestrictedCourse;
using SFA.DAS.Roatp.Application.RestrictedCourses.Queries.GetAllRestrictedCourses;
using static SFA.DAS.Roatp.Api.Infrastructure.Constants;

namespace SFA.DAS.Roatp.Api.Controllers;

[ApiController]
[ApiVersion(ApiVersionNumber.One)]
[Route("/restricted-courses")]
public class RestrictedCoursesController(IMediator _mediator, ILogger<RestrictedCoursesController> _logger) : ActionResponseControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(GetAllRestrictedCoursesQueryResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRestrictedCourses([FromQuery] bool restricted)
    {
        _logger.LogInformation("Request received to get restricted courses");

        GetAllRestrictedCoursesQuery query = new(restricted);
        GetAllRestrictedCoursesQueryResult result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> CreateRestrictedCourse(AddRestrictedCourseCommand command)
    {
        _logger.LogInformation("Request to create restricted course for {LarsCode}", command.LarsCode);

        var response = await _mediator.Send(command);

        return GetNoContentResponse(response);
    }
}
