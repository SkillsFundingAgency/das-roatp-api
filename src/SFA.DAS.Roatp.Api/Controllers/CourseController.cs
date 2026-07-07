using System.Threading.Tasks;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Api.Infrastructure;
using SFA.DAS.Roatp.Application.Common.Models;
using SFA.DAS.Roatp.Application.Course.GetAllowedProviders.Queries;
using SFA.DAS.Roatp.Application.Course.GetProvidersNotAllowed.Queries;
using static SFA.DAS.Roatp.Api.Infrastructure.Constants;

namespace SFA.DAS.Roatp.Api.Controllers;

[ApiController]
[ApiVersion(ApiVersionNumber.One)]
[Route("/courses")]

public class CourseController(IMediator _mediator, ILogger<CourseController> _logger) : ActionResponseControllerBase
{
    [HttpGet("{larsCode}/providers/allowed")]
    [ProducesResponseType(typeof(CourseAllowedProvidersModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAllowedProvidersByCourse([FromRoute] string larsCode)
    {
        _logger.LogInformation("Request received to get allowed providers for LarsCode: {LarsCode}", larsCode);

        GetAllowedProvidersQuery query = new(larsCode);
        var result = await _mediator.Send(query);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpGet("{larsCode}/providers/not-allowed")]
    [ProducesResponseType(typeof(CourseAllowedProvidersModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProvidersNotAllowedByCourse([FromRoute] string larsCode)
    {
        _logger.LogInformation("Request received to get providers not allowed for LarsCode: {LarsCode}", larsCode);

        GetProvidersNotAllowedQuery query = new(larsCode);
        var result = await _mediator.Send(query);
        return result is null ? NotFound() : Ok(result);
    }
}
