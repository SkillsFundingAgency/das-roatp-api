using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Api.Models;
using SFA.DAS.Roatp.Application.Locations.Commands.BulkInsert;

namespace SFA.DAS.Roatp.Api.Controllers
{
    [ApiController]
    public class ProviderLocationsBulkInsertController : Controller
    {
        private readonly ILogger<ProviderLocationsBulkInsertController> _logger;
        private readonly IMediator _mediator;

        public ProviderLocationsBulkInsertController(ILogger<ProviderLocationsBulkInsertController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [HttpPost]
        [Route("/providers/{ukprn}/locations/bulk-insert-regions")]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status204NoContent)]
        public async Task<ActionResult> BulkInsertProviderLocations([FromRoute] int ukprn,  ProviderLocationsInsertModel providerLocationsInsertModel)
        {
            _logger.LogInformation("Inner API: Request received to bulk insert locations ukprn: {ukprn} larscode: {larscode} userid:{userid}", ukprn, providerLocationsInsertModel.LarsCode, providerLocationsInsertModel.UserId);
            var command = (BulkInsertProviderLocationsCommand)providerLocationsInsertModel;
            command.Ukprn = ukprn;
            var result =  await _mediator.Send(command);
            _logger.LogInformation("Inserted {numberOfRecordsInserted} provider locations for Ukprn:{ukprn} LarsCode:{larscode}", result, ukprn, providerLocationsInsertModel.LarsCode);

            return NoContent();
        }
    }
}
