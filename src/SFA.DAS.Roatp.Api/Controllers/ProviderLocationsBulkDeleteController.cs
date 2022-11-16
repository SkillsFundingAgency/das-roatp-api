using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Application.Locations.Commands.BulkDelete;

namespace SFA.DAS.Roatp.Api.Controllers
{
    [ApiController]
    public class ProviderLocationsBulkDeleteController : Controller
    {
        private readonly ILogger<ProviderLocationsBulkDeleteController> _logger;
        private readonly IMediator _mediator;

        public ProviderLocationsBulkDeleteController(ILogger<ProviderLocationsBulkDeleteController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [HttpDelete]
        [Route("/providers/{ukprn}/locations/cleanup")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> BulkDeleteProviderLocations([FromRoute] int ukprn, [FromQuery] string userId, [FromQuery] string userDisplayName)
        {
            _logger.LogInformation("Inner API: Request received to bulk delete provider locations ukprn: {ukprn}  userid:{userid}", ukprn, userId);
            
            var command = new BulkDeleteProviderLocationsCommand(ukprn, userId, userDisplayName);
            var result = await _mediator.Send(command);

            _logger.LogInformation("Deleted {numberOfRecordsDeleted} provider locations for Ukprn:{ukprn}", result, ukprn);

            return NoContent();
        }
    }
}
