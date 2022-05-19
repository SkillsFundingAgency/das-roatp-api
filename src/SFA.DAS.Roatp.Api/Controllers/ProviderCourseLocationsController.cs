using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Application.ProviderCourseLocations.Queries;

namespace SFA.DAS.Roatp.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProviderCourseLocationsController : ControllerBase
    {
        private readonly ILogger<ProviderCourseLocationsController> _logger;
        private readonly IMediator _mediator;

        public ProviderCourseLocationsController(ILogger<ProviderCourseLocationsController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [HttpGet]
        [Route("/providerCourseLocations/{providerCourseId}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(List<ProviderCourseLocationModel>), 200)]
        public async Task<ActionResult<List<ProviderCourseLocationModel>>> GetProviderCourseLocations(int providerCourseId)
        {
            _logger.LogInformation("Request received to get all provider Course Locations for providerCourseId: {providerCourseId}", providerCourseId);

            var result = await _mediator.Send(new ProviderCourseLocationsQuery(providerCourseId));

            _logger.LogInformation("Found {locationCount} locations for {providerCourseId}", result.ProviderCourseLocations.Count, providerCourseId);

            return new OkObjectResult(result.ProviderCourseLocations);
        }
    }
}
