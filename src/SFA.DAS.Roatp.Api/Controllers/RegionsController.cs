using System.Collections.Generic;
using System.Threading.Tasks;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Application.Regions.Queries;

namespace SFA.DAS.Roatp.Api.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]

    public class RegionsController : Controller
    {
        private readonly ILogger<RegionsController> _logger;
        private readonly IMediator _mediator;

        public RegionsController(ILogger<RegionsController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [HttpGet]
        [Route("/lookup/regions")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(List<RegionModel>), 200)]
        public async Task<ActionResult<List<RegionModel>>> GetRegions()
        {
            _logger.LogInformation("Request received to get all Regions");

            var result = await _mediator.Send(new RegionsQuery());

            _logger.LogInformation("Found {regionsCount} Regions", result.Regions.Count);

            return new OkObjectResult(result.Regions);
        }
    }
}
