using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Api.Models;
using SFA.DAS.Roatp.Application.ProviderCourseLocations.Commands.BulkInsert;

namespace SFA.DAS.Roatp.Api.Controllers
{
    [ApiController]
    public class ProviderCourseLocationsBulkInsertController : Controller
    {
        private readonly ILogger<ProviderCourseLocationsBulkInsertController> _logger;
        private readonly IMediator _mediator;

        public ProviderCourseLocationsBulkInsertController(ILogger<ProviderCourseLocationsBulkInsertController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [HttpPost]
        [Route("/providers/{ukprn}/courses/{larsCode}/locations/bulk-insert-regions")]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status204NoContent)]
        public async Task<IActionResult> BulkInsertProviderCourseLocations([FromRoute] int ukprn, ProviderCourseLocationsInsertModel providerLocationsInsertModel)
        {
            _logger.LogInformation("Inner API: Request received to bulk insert provider course locations ukprn: {ukprn} larscode: {larscode} userid:{userid}", ukprn, providerLocationsInsertModel.LarsCode, providerLocationsInsertModel.UserId);
            var command = (BulkInsertProviderCourseLocationsCommand)providerLocationsInsertModel;
            command.Ukprn = ukprn;
            var result = await _mediator.Send(command);
            _logger.LogInformation("Inserted {numberOfRecordsInserted} provider course locations for Ukprn:{ukprn} LarsCode:{larscode}", result, ukprn, providerLocationsInsertModel.LarsCode);

            return NoContent();
        }
    }
}
