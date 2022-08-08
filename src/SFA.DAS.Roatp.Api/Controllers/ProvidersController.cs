using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Application.Provider.Queries.GetProvider;

namespace SFA.DAS.Roatp.Api.Controllers
{
    [ApiController]
    public class ProvidersController : Controller
    {
        private readonly ILogger<ProvidersController> _logger;
        private readonly IMediator _mediator;

        public ProvidersController(ILogger<ProvidersController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [HttpGet]
        [Route("/providers/{ukprn}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ProviderModel>> GetProvider(int ukprn)
        {
            var providerResult = await _mediator.Send(new GetProviderQuery(ukprn));
            var result = providerResult.Provider;
            _logger.LogInformation("Provider data found for ukprn [{ukprn}]", ukprn);
            return new OkObjectResult(result);
        }
    }
}
