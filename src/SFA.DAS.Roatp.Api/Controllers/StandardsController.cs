using System.Net;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Api.Models;

namespace SFA.DAS.Roatp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StandardsController : Controller
    {
        private readonly IMediator _mediator;
        private readonly ILogger<StandardsController> _logger;

        public StandardsController(IMediator mediator, ILogger<StandardsController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }


        [HttpPost]
        [Route("/ReloadStandardsData")]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpStatusCode), 200)]
        public async Task<IActionResult> ReloadStandardsData(StandardsRequest request)
        {
            var reloadStandardRequest = new ReloadStandardsRequest { Standards = request.Standards };
            var successful= await _mediator.Send(reloadStandardRequest);

            if (!successful)
            {
                _logger.LogWarning("ReloadStandardsData processing failed");
                return BadRequest();
            }

            _logger.LogInformation("ReloadStandardsData processing completed");
            return Ok();
        }
    }
}
