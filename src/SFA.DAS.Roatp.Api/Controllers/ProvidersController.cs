using System.Threading.Tasks;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Api.Infrastructure;
using SFA.DAS.Roatp.Application.Providers.Queries.GetProvider;
using static SFA.DAS.Roatp.Api.Infrastructure.Constants;

namespace SFA.DAS.Roatp.Api.Controllers;

[ApiController]
[ApiVersion(ApiVersionNumber.One)]
[Tags(EndpointTags.Providers)]
[Route("providers/{ukprn:int}")]
public class ProvidersController : ActionResponseControllerBase
{
    private readonly ILogger<ProvidersController> _logger;
    private readonly IMediator _mediator;

    public ProvidersController(ILogger<ProvidersController> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    [HttpGet]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(GetProviderQueryResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProvider([FromRoute] int ukprn)
    {
        var response = await _mediator.Send(new GetProviderQuery(ukprn));
        _logger.LogInformation("Provider data found for ukprn: {Ukprn}", ukprn);

        return GetResponse(response);
    }
}
