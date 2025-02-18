using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Api.Infrastructure;
using SFA.DAS.Roatp.Application.Courses.Queries.GetCourseTrainingProvidersCount;
using SFA.DAS.Roatp.Application.Courses.Queries.GetProviderDetailsForCourse;
using System.Threading;
using SFA.DAS.Roatp.Application.Courses.Queries.GetProvidersFromLarsCode;
using SFA.DAS.Roatp.Application.ProviderCourse.Queries.GetProvidersCountForCourse;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Api.Controllers.ExternalReadControllers;

[ApiController]
[Route("/api/[controller]/")]
public class CoursesController : ActionResponseControllerBase
{
    private readonly ILogger<CoursesController> _logger;
    private readonly IMediator _mediator;

    public CoursesController(IMediator mediator, ILogger<CoursesController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet("providers/count")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(GetCourseTrainingProvidersCountQueryResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTrainingProvidersCount([FromQuery] GetCourseTrainingProvidersCountQuery query, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(query, cancellationToken);
        return GetResponse(response);
    }

    [HttpGet]
    [Route("{larsCode}/providers/{ukprn}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(GetProviderDetailsForCourseQueryResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProviderDetailsForCourse(int larsCode, int ukprn, decimal? lat = null, decimal? lon = null)
    {
        _logger.LogInformation("Received request to get provider details for Ukprn: {Ukprn}, LarsCode: {LarsCode},  Latitude: {Latitude}, Long: {Longitude}", ukprn, larsCode, lat, lon);
        var response = await _mediator.Send(new GetProviderDetailsForCourseQuery(larsCode, ukprn, lat, lon));
        return GetResponse(response);
    }

    [HttpGet]
    [Route("{larsCode}/providers")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(GetProvidersForLarsCodeQueryResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProvidersForLarsCode([FromRoute] int larsCode, [FromQuery] GetProvidersFromLarsCodeRequest request)
    {
        _logger.LogInformation("Received request to get list of providers for LarsCode: {LarsCode},  Latitude: {Latitude}, Longitude: {Longitude}", larsCode, request.Latitude, request.Longitude);
        var response = await _mediator.Send(new GetProvidersForLarsCodeQuery(larsCode, request));
        return GetResponse(response);
    }


}
