using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Application.ImportProvider;

namespace SFA.DAS.Roatp.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ImportProviderController : ControllerBase
    {

        private readonly IMediator _mediator;
        private readonly ILogger<ImportProviderController> _logger;

        public ImportProviderController(IMediator mediator, ILogger<ImportProviderController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpPost]
        [Route("/ImportProvider")]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(HttpStatusCode), 200)]
        public async Task<IActionResult> ImportProvider(ImportProviderRequest request) 
        {
            var ukprn = request.Provider.Ukprn;
            _logger.LogInformation("Import provider processing started for {ukprn}",ukprn);
            var successful = await _mediator.Send(request);

            if (!successful)
            {
                _logger.LogWarning("Import provider processing failed for ukprn: {ukprn}",ukprn);
                return BadRequest();
            }

            _logger.LogInformation("Import provider processing completed for ukprn: {ukprn}",ukprn);
            return Ok();
        }
    }
}