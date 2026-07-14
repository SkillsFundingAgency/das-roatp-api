using System.Threading;
using System.Threading.Tasks;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Api.Infrastructure;
using SFA.DAS.Roatp.Application.ProviderAllowedCourses.Commands.UpsertProviderAllowedCourse;
using SFA.DAS.Roatp.Application.ProviderAllowedCourses.Queries.GetProviderAllowedCourses;
using SFA.DAS.Roatp.Domain.Models;
using static SFA.DAS.Roatp.Api.Infrastructure.Constants;

namespace SFA.DAS.Roatp.Api.Controllers;

[ApiController]
[ApiVersion(ApiVersionNumber.One)]
[Tags(EndpointTags.ProviderAllowedCourses)]
[Route("providers/{ukprn}/allowed-courses")]
public class ProviderAllowedCoursesController(IMediator _mediator, ILogger<ProviderAllowedCoursesController> _logger) : ActionResponseControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(GetProviderAllowedCoursesQueryResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllowedCourses([FromRoute] int ukprn, [FromQuery] CourseType courseType = CourseType.ShortCourse, CancellationToken cancellationToken = default)
    {
        GetProviderAllowedCoursesQuery query = new(ukprn, courseType);
        GetProviderAllowedCoursesQueryResult result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpPost("{larsCode}")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> UpsertProviderAllowedCourse([FromRoute] int ukprn, [FromRoute] string larsCode, [FromBody] UpsertProviderAllowedCourseModel request)
    {
        _logger.LogInformation("Request to upsert provider allowed course for Ukprn {Ukprn} and LarsCode {LarsCode}", ukprn, larsCode);

        UpsertProviderAllowedCourseCommand command = new()
        {
            Ukprn = ukprn,
            LarsCode = larsCode,
            UserId = request.UserId,
            UserDisplayName = request.UserDisplayName,
            LastDateStarts = request.LastDateStarts,
        };

        var response = await _mediator.Send(command);

        return GetNoContentResponse(response);
    }
}
