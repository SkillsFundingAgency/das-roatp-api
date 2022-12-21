using MediatR;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Application.Providers.Commands.PatchProvider;
using SFA.DAS.Roatp.Domain.Models;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Api.Controllers
{
    [ApiController]
    public class ProviderEditController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ProviderEditController> _logger;

        public ProviderEditController(IMediator mediator, ILogger<ProviderEditController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [Route("/providers/{ukprn}")]
        [HttpPatch] 
        public async Task<IActionResult> PatchProvider([FromRoute] int ukprn, [FromBody] JsonPatchDocument<PatchProvider> request, [FromQuery] string userId, [FromQuery] string userDisplayName)
        {
            _logger.LogInformation("Inner API: Request to patch provider for ukprn: {ukprn}", ukprn);

            await _mediator.Send(new PatchProviderCommand
            {
                Ukprn = ukprn,
                UserId = userId,
                UserDisplayName = userDisplayName,
                Patch = request
            });
            
            return NoContent();
        }
    }
}
