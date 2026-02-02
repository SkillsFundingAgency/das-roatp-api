using System.Threading.Tasks;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Api.Infrastructure;
using SFA.DAS.Roatp.Application.Providers.Queries.GetProviderRegisterDetailsLookup;
using static SFA.DAS.Roatp.Api.Infrastructure.Constants;

namespace SFA.DAS.Roatp.Api.Controllers.ExternalReadControllers;

[ApiController]
[ApiVersion(ApiVersionNumber.Two)]
[Route("/api/[controller]")]
public class ExportController : ActionResponseControllerBase
{
    private readonly ILogger<ExportController> _logger;
    private readonly IMediator _mediator;

    public ExportController(ILogger<ExportController> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    [HttpGet]
    [Route("providers/{ukprn}")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(GetProviderRegisterDetailsLookupQueryResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProviderDetailsForUkprn([FromRoute] int ukprn)
    {
        _logger.LogInformation("Inner API: Request received to get provider details for ukprn {Ukprn}", ukprn);
        var result = await _mediator.Send(new GetProviderRegisterDetailsLookupQuery(ukprn));
        return GetResponse(result);
    }
}
