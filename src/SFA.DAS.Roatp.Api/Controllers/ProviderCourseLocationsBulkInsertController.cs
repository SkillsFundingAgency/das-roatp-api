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
        [Route("/providers/{ukprn}/locations/bulk2")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(List<ProviderLocationModel>), 200)]
        public async Task<ActionResult<List<ProviderLocationModel>>> BulkInsertProviderLocations([FromRoute] int ukprn,  ProviderLocationsInsertModel providerLocationsInsertModel)
        {
            _logger.LogInformation("Inner API: Request received to bulk insert locations ukprn: {ukprn} larscode: {larscode} userid:{userid}", ukprn, providerLocationsInsertModel.LarsCode, providerLocationsInsertModel.UserId);
            var command = (BulkInsertProviderLocationsCommand)providerLocationsInsertModel;
            command.Ukprn = ukprn;
            await _mediator.Send(command);
            return NoContent();
        }
    }
}
