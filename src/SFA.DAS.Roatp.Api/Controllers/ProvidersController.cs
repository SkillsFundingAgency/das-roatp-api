using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Api.Infrastructure;
using SFA.DAS.Roatp.Application.Providers.Queries.GetProvider;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Api.Controllers
{
    [ApiController]
    public class ProvidersController : ActionResponseControllerBase
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
        [ProducesResponseType(typeof(GetProviderQueryResult), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetProvider(int ukprn)
        {
            var response = await _mediator.Send(new GetProviderQuery(ukprn));
            _logger.LogInformation("Provider data found for ukprn [{ukprn}]", ukprn);

            return GetResponse(response);
        }
    }
}
