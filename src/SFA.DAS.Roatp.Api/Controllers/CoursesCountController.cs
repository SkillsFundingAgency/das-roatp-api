using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Roatp.Api.Infrastructure;
using SFA.DAS.Roatp.Application.Courses.Queries.GetCourseTrainingProvidersCount;

namespace SFA.DAS.Roatp.Api.Controllers;

[ApiController]
[Route("api/Courses/providers/count")]
public class CoursesCountController(IMediator _mediator) : ActionResponseControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(GetCourseTrainingProvidersCountQueryResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTrainingProvidersCount([FromQuery] GetCourseTrainingProvidersCountQuery query, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(query, cancellationToken);
        return GetResponse(response);
    }
}
