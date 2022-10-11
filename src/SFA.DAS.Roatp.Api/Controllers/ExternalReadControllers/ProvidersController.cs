using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Application.Providers.Queries.GetProviders;
using SFA.DAS.Roatp.Application.Providers.Queries.GetProviderSummary;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Api.Controllers.ExternalReadControllers
{
    [ApiController]
    [Route("/api/[controller]/")]
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
        [Route("")]
        public async Task<IActionResult> GetProviders()
        {
            var providerResult = await _mediator.Send(new GetProvidersQuery());
            _logger.LogInformation("Providers summary data found");
            return new OkObjectResult(providerResult);
        }

        [HttpGet]
        [Route("{ukprn}")]
        public async Task<IActionResult> GetProvider(int ukprn)
        {
            var providerResult = await _mediator.Send(new GetProviderSummaryQuery(ukprn));
            _logger.LogInformation("Provider summary data found for {ukprn}:", ukprn);
            return new OkObjectResult(providerResult);
        }
    }
}
