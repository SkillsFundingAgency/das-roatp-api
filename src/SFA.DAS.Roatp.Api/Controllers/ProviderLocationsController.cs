using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Api.Infrastructure;
using SFA.DAS.Roatp.Application.Locations.Queries.GetProviderLocationDetails;
using SFA.DAS.Roatp.Application.Locations.Queries.GetProviderLocations;
using static SFA.DAS.Roatp.Api.Infrastructure.Constants;

namespace SFA.DAS.Roatp.Api.Controllers;

[ApiController]
[ApiVersion(ApiVersionNumber.One)]
[Tags(EndpointTags.ProviderLocations)]
[Route("/providers/{ukprn}/locations")]
public class ProviderLocationsController : ActionResponseControllerBase
{
    private readonly ILogger<ProviderLocationsController> _logger;
    private readonly IMediator _mediator;

    public ProviderLocationsController(ILogger<ProviderLocationsController> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    [HttpGet]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(List<ProviderLocationModel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetLocations(int ukprn)
    {
        _logger.LogInformation("Request received to get all locations for ukprn: {Ukprn}", ukprn);

        var response = await _mediator.Send(new GetProviderLocationsQuery(ukprn));

        if (response.IsValidResponse)
            _logger.LogInformation("Found {LocationCount} locations for {Ukprn}", response.Result.Count, ukprn);

        return GetResponse(response);
    }

    [HttpGet]
    [Route("{id}")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProviderLocationModel), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetLocation([FromRoute] int ukprn, [FromRoute] Guid id)
    {
        _logger.LogInformation("Request received to get provider location details for ukprn: {Ukprn} and {Id}", ukprn, id);

        var response = await _mediator.Send(new GetProviderLocationDetailsQuery(ukprn, id));

        return response.Errors.Any(x => x.ErrorMessage == GetProviderLocationDetailsQueryValidator.ProviderLocationNotFoundErrorMessage) ?
            NotFound() :
            GetResponse(response);
    }
}
