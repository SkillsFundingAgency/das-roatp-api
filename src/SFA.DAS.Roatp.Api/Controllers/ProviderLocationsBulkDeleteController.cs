using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Api.Models;
using SFA.DAS.Roatp.Application.Locations.Commands.BulkInsert;
using SFA.DAS.Roatp.Application.Locations.Queries;

namespace SFA.DAS.Roatp.Api.Controllers
{
    [ApiController]
    [Route("[controller]/")]
    public class ProviderLocationsBulkDeleteController : Controller
    {
        private readonly ILogger<ProviderLocationsBulkDeleteController> _logger;
        private readonly IMediator _mediator;

        public ProviderLocationsBulkDeleteController(ILogger<ProviderLocationsBulkDeleteController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [HttpPost]
        [Route("/providers/{ukprn}/locations/cleanup")]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status204NoContent)]
        public async Task<ActionResult<List<ProviderLocationModel>>> BulkDeleteProviderLocations([FromRoute] int ukprn,  ProviderLocationsInsertModel providerLocationsInsertModel)
        {
            _logger.LogInformation("Inner API: Request received to bulk insert locations ukprn: {ukprn} larscode: {larscode} userid:{userid}", ukprn, providerLocationsInsertModel.LarsCode, providerLocationsInsertModel.UserId);
            var command = (BulkInsertProviderLocationsCommand)providerLocationsInsertModel;
            command.Ukprn = ukprn;
            await _mediator.Send(command);
            return NoContent();
        }
    }
}
