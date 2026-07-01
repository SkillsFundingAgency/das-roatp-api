using System.Threading.Tasks;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Api.Infrastructure;
using SFA.DAS.Roatp.Application.Course.GetAllowedProviders.Queries;
using static SFA.DAS.Roatp.Api.Infrastructure.Constants;

namespace SFA.DAS.Roatp.Api.Controllers;

[ApiController]
[ApiVersion(ApiVersionNumber.One)]
[Route("/courses")]

public class CourseController(IMediator _mediator, ILogger<CourseController> _logger) : ActionResponseControllerBase
{
    [HttpGet("{larsCode}/providers/allowed")]
    [ProducesResponseType(typeof(GetAllowedProvidersQueryResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllowedProvidersByCourse([FromRoute] string larsCode)
    {
        _logger.LogInformation("Request received to get allowed providers for a course");

        GetAllowedProvidersQuery query = new(larsCode);
        var result = await _mediator.Send(query);
        return GetResponse(result);
    }
}
