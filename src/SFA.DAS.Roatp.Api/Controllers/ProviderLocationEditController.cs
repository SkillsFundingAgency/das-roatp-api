using System;
using System.Threading.Tasks;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Api.Infrastructure;
using SFA.DAS.Roatp.Api.Models;
using SFA.DAS.Roatp.Application.Locations.Commands.UpdateProviderLocationDetails;
using static SFA.DAS.Roatp.Api.Infrastructure.Constants;

namespace SFA.DAS.Roatp.Api.Controllers;

[ApiController]
[ApiVersion(ApiVersionNumber.One)]
[Tags(EndpointTags.ProviderLocations)]
[Route("/providers/{ukprn}/locations/{id}")]
public class ProviderLocationEditController : ActionResponseControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<ProviderLocationEditController> _logger;

    public ProviderLocationEditController(IMediator mediator, ILogger<ProviderLocationEditController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }


    [HttpPut]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Save([FromRoute] int ukprn, [FromRoute] Guid id, ProviderLocationEditModel providerLocationEditModel)
    {
        _logger.LogInformation("Inner API: Request to update provider location details for ukprn: {Ukprn} id: {Id} userid:{Userid}", ukprn, id, providerLocationEditModel.UserId);
        var command = (UpdateProviderLocationDetailsCommand)providerLocationEditModel;
        command.Ukprn = ukprn;
        command.Id = id;
        var response = await _mediator.Send(command);

        return GetNoContentResponse(response);
    }
}
