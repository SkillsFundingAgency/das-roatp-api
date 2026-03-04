using System.Threading.Tasks;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Api.Infrastructure;
using SFA.DAS.Roatp.Api.Models;
using SFA.DAS.Roatp.Application.Locations.Commands.BulkInsert;
using static SFA.DAS.Roatp.Api.Infrastructure.Constants;

namespace SFA.DAS.Roatp.Api.Controllers;

[ApiController]
[ApiVersion(ApiVersionNumber.One)]
[Tags(EndpointTags.ProviderLocations)]
[Route("/providers/{ukprn}/locations/bulk-insert-regions")]
public class ProviderLocationsBulkInsertController : ActionResponseControllerBase
{
    private readonly ILogger<ProviderLocationsBulkInsertController> _logger;
    private readonly IMediator _mediator;

    public ProviderLocationsBulkInsertController(ILogger<ProviderLocationsBulkInsertController> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> BulkInsertProviderLocations([FromRoute] int ukprn, ProviderLocationsInsertModel providerLocationsInsertModel)
    {
        _logger.LogInformation("Inner API: Request received to bulk insert locations ukprn: {Ukprn} larscode: {LarsCode} userid:{UserId}", ukprn, providerLocationsInsertModel.LarsCode, providerLocationsInsertModel.UserId);
        var command = (BulkInsertProviderLocationsCommand)providerLocationsInsertModel;
        command.Ukprn = ukprn;
        var response = await _mediator.Send(command);
        if (response.IsValidResponse)
            _logger.LogInformation("Inserted {NumberOfRecordsInserted} provider locations for Ukprn:{Ukprn} LarsCode:{LarsCode}", response.Result, ukprn, providerLocationsInsertModel.LarsCode);

        return GetNoContentResponse(response);
    }
}
