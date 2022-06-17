using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Application.Region.Queries;

namespace SFA.DAS.Roatp.Api.Controllers
{
    [ApiController]
    [Route("[controller]/")]
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
        [Route("/regions")]
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
