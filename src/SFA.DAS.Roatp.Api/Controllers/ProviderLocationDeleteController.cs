using System;
using System.Threading.Tasks;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Api.Infrastructure;
using SFA.DAS.Roatp.Application.Locations.Commands.DeleteLocation;
using static SFA.DAS.Roatp.Api.Infrastructure.Constants;

namespace SFA.DAS.Roatp.Api.Controllers;

[ApiVersion(ApiVersionNumber.One)]

[Route("/providers/{ukprn}/locations/{id}")]
public class ProviderLocationDeleteController : ActionResponseControllerBase
{
    private readonly ILogger<ProviderLocationDeleteController> _logger;
    private readonly IMediator _mediator;


    public ProviderLocationDeleteController(IMediator mediator, ILogger<ProviderLocationDeleteController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteProviderLocation([FromRoute] int ukprn, [FromRoute] Guid id, [FromQuery] string userId, [FromQuery] string UserDisplayName)
    {
        _logger.LogInformation("Inner API: Request received to delete provider location ukprn: {ukprn} Id: {id} userid:{userId}", ukprn, id, userId);

        var command = new DeleteProviderLocationCommand(ukprn, id, userId, UserDisplayName);
        var response = await _mediator.Send(command);

        switch (response)
        {
            case { Result: false, IsValidResponse: true }:
                _logger.LogInformation("Deleted provider location for Ukprn:{ukprn} id:{id} has no matching id", ukprn, id);
                return NotFound();
            default:
                return GetNoContentResponse(response);
        }
    }
}
