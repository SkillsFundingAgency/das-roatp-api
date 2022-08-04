using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Application.Standards.Queries;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Api.Controllers
{
    [ApiController]
    public class StandardsController : ControllerBase
    {
        private readonly ILogger<StandardsController> _logger;
        private readonly IMediator _mediator;

        public StandardsController(ILogger<StandardsController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [HttpGet]
        [Route("standards")]
        [Produces("application/json")]
        public async Task<ActionResult<GetAllStandardsQueryResult>> GetAllStandards()
        {
            _logger.LogInformation("Inner API: Request received to get all standards");
            var result = await _mediator.Send(new GetAllStandardsQuery());
            return Ok(result);
        }
    }
}
