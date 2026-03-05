using System.Collections.Generic;
using System.Threading.Tasks;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Application.Regions.Queries;
using static SFA.DAS.Roatp.Api.Infrastructure.Constants;

namespace SFA.DAS.Roatp.Api.Controllers;

[ApiController]
[ApiVersion(ApiVersionNumber.One)]
[Tags(EndpointTags.Lookups)]
[Route("/lookup/regions")]
public class RegionsController : ControllerBase
{
    private readonly ILogger<RegionsController> _logger;
    private readonly IMediator _mediator;

    public RegionsController(ILogger<RegionsController> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    [HttpGet]
    [Produces("application/json")]
    [ProducesResponseType(typeof(List<RegionModel>), 200)]
    public async Task<ActionResult<List<RegionModel>>> GetRegions()
    {
        _logger.LogInformation("Request received to get all Regions");

        var result = await _mediator.Send(new RegionsQuery());

        _logger.LogInformation("Found {RegionsCount} Regions", result.Regions.Count);

        return new OkObjectResult(result.Regions);
    }
}
