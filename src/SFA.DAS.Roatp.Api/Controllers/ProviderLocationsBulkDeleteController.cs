using System.Threading.Tasks;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Api.Infrastructure;
using SFA.DAS.Roatp.Application.Locations.Commands.BulkDelete;
using static SFA.DAS.Roatp.Api.Infrastructure.Constants;

namespace SFA.DAS.Roatp.Api.Controllers
{
    [ApiController]
    [ApiVersion(ApiVersionNumber.One)]

    public class ProviderLocationsBulkDeleteController : ActionResponseControllerBase
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
        public async Task<IActionResult> BulkDeleteProviderLocations([FromRoute] int ukprn, [FromQuery] string userId, [FromQuery] string userDisplayName)
        {
            _logger.LogInformation("Inner API: Request received to bulk delete provider locations ukprn: {ukprn}  userid:{userid}", ukprn, userId);

            var command = new BulkDeleteProviderLocationsCommand(ukprn, userId, userDisplayName);
            var response = await _mediator.Send(command);

            if (response.IsValidResponse)
                _logger.LogInformation("Deleted {numberOfRecordsDeleted} provider locations for Ukprn:{ukprn}", response.Result, ukprn);

            return GetNoContentResponse(response);
        }
    }
}
