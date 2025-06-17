using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Api.Infrastructure;
using SFA.DAS.Roatp.Application.Locations.Queries.GetProviderLocationDetails;
using SFA.DAS.Roatp.Application.Locations.Queries.GetProviderLocations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Api.Controllers
{
    [ApiController]
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
        [Route("/providers/{ukprn}/locations")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(List<ProviderLocationModel>), 200)]
        public async Task<IActionResult> GetLocations(int ukprn)
        {
            _logger.LogInformation("Request received to get all locations for ukprn: {ukprn}", ukprn);

            var response = await _mediator.Send(new GetProviderLocationsQuery(ukprn));

            if (response.IsValidResponse)
                _logger.LogInformation("Found {locationCount} locations for {ukprn}", response.Result.Count, ukprn);

            return GetResponse(response);
        }

        [HttpGet]
        [Route("/providers/{ukprn}/locations/{id}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProviderLocationModel), 200)]
        public async Task<IActionResult> GetLocation([FromRoute] int ukprn, [FromRoute] Guid id)
        {
            _logger.LogInformation("Request received to get provider location details for ukprn: {ukprn} and {id}", ukprn, id);

            var response = await _mediator.Send(new GetProviderLocationDetailsQuery(ukprn, id));

            return response.Errors.Any(x => x.ErrorMessage == GetProviderLocationDetailsQueryValidator.ProviderLocationNotFoundErrorMessage) ?
                NotFound() :
                GetResponse(response);
        }
    }
}
