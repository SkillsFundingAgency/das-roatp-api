using System.Threading.Tasks;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Api.Infrastructure;
using SFA.DAS.Roatp.Application.Courses.Queries.GetCourseProviderDetails;
using SFA.DAS.Roatp.Application.Courses.Queries.GetProvidersFromLarsCode;
using static SFA.DAS.Roatp.Api.Infrastructure.Constants;

namespace SFA.DAS.Roatp.Api.Controllers.ExternalReadControllers;

[ApiController]
[ApiVersion(ApiVersionNumber.One)]
[ApiVersion(ApiVersionNumber.Two)]
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

    [HttpGet]
    [MapToApiVersion(ApiVersionNumber.One)]
    [Route("{larsCode}/providers")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(GetProvidersForLarsCodeQueryResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProvidersForLarsCode([FromRoute] string larsCode, [FromQuery] GetProvidersFromLarsCodeRequest request)
    {
        _logger.LogInformation("Received request to get list of providers for LarsCode: {LarsCode},  Latitude: {Latitude}, Longitude: {Longitude}", larsCode, request.Latitude, request.Longitude);
        var response = await _mediator.Send(new GetProvidersForLarsCodeQuery(larsCode, request));
        return GetResponse(response);
    }

    [HttpGet]
    [MapToApiVersion(ApiVersionNumber.One)]
    [Route("{larsCode:int}/providers/{ukprn:int}/details")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(GetCourseProviderDetailsResultV1Model), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCourseProviderDetails([FromRoute] int larsCode, [FromRoute] int ukprn, [FromQuery] GetCourseProviderDetailsRequest request)
    {
        var courseProviderDetails =
            await _mediator.Send(
                new GetCourseProviderDetailsQuery(
                    ukprn,
                    larsCode.ToString(),
                    request.ShortlistUserId,
                    request.Location,
                    request.Longitude,
                    request.Latitude
                    )
                );
        if (courseProviderDetails == null)
            return NotFound();

        return GetResponse(courseProviderDetails);
    }

    [HttpGet]
    [MapToApiVersion(ApiVersionNumber.Two)]
    [Route("{larsCode}/providers/{ukprn:int}/details")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(GetCourseProviderDetailsQueryResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCourseProviderDetails([FromRoute] string larsCode, [FromRoute] int ukprn, [FromQuery] GetCourseProviderDetailsRequest request)
    {
        var courseProviderDetails =
            await _mediator.Send(
                new GetCourseProviderDetailsQuery(
                    ukprn,
                    larsCode,
                    request.ShortlistUserId,
                    request.Location,
                    request.Longitude,
                    request.Latitude
                    )
                );
        if (courseProviderDetails == null)
            return NotFound();

        return GetResponse(courseProviderDetails);
    }
}
