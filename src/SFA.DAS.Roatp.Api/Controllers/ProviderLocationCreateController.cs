using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Api.Models;
using SFA.DAS.Roatp.Application.Locations.Commands.CreateLocation;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Api.Controllers
{
    [ApiController]
    public class ProviderLocationCreateController : ControllerBase
    {
        private readonly ILogger<ProviderLocationCreateController> _logger;
        private readonly IMediator _mediator;

        public ProviderLocationCreateController(IMediator mediator, ILogger<ProviderLocationCreateController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpPost]
        [Route("/providers/{ukprn}/locations")]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateLocation([FromRoute] int ukprn, ProviderLocationCreateModel model)
        {
            _logger.LogInformation("Request to save Provider Location {locationName} for Ukprn {ukprn}", model.LocationName, ukprn);

            var command = (CreateProviderLocationCommand)model;
            command.Ukprn = ukprn;

            await _mediator.Send(command);

            return new StatusCodeResult(StatusCodes.Status201Created);
        }
    }
}
