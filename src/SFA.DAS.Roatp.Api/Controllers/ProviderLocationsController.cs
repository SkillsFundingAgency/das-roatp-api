using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Application.Locations.Queries;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Api.Controllers
{
    [ApiController]
    public class ProviderLocationsController : ControllerBase
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

            var result = await _mediator.Send(new ProviderLocationsQuery(ukprn));

            _logger.LogInformation("Found {locationCount} locations for {ukprn}", result.Locations.Count, ukprn);

            return new OkObjectResult(result.Locations);
        }
    }
}
