using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Application.Locations.Queries.GetProviderLocationDetails;
using SFA.DAS.Roatp.Application.Locations.Queries.GetProviderLocations;

namespace SFA.DAS.Roatp.Api.Controllers
{
    [ApiController]
    [Route("[controller]/")]
    public class ProviderLocationsController : Controller
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
        public async Task<ActionResult<List<ProviderLocationModel>>> GetLocations(int ukprn)
        {
            _logger.LogInformation("Request received to get all locations for ukprn: {ukprn}", ukprn);

            var result = await _mediator.Send(new GetProviderLocationsQuery(ukprn));

            _logger.LogInformation("Found {locationCount} locations for {ukprn}", result.Locations.Count, ukprn);

            return new OkObjectResult(result.Locations);
        }

        [HttpGet]
        [Route("/providers/{ukprn}/locations/{id}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProviderLocationModel), 200)]
        public async Task<ActionResult<ProviderLocationModel>> GetLocation([FromRoute] int ukprn, [FromRoute] Guid id)
        {
            _logger.LogInformation("Request received to get provider location details for ukprn: {ukprn} and {id}", ukprn, id);

            var result = await _mediator.Send(new GetProviderLocationDetailsQuery(ukprn, id));

            _logger.LogInformation("Found provider location details for {ukprn} and {id}", ukprn, id);

            return new OkObjectResult(result.Location);
        }
    }
}
