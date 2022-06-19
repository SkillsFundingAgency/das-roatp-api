using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Application.ProviderCourseLocations.Commands.BulkDelete;
using System.Net;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Api.Controllers
{
    [ApiController]
    public class DeleteProviderCourseLocationsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<DeleteProviderCourseLocationsController> _logger;

        public DeleteProviderCourseLocationsController(IMediator mediator, ILogger<DeleteProviderCourseLocationsController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpDelete]
        [Route("/providers/{ukprn}/courses/{larsCode}/locations")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> BulkDeleteProviderCourseLocations([FromRoute] int ukprn, [FromRoute] int larsCode, [FromQuery] DeleteProviderCourseLocationOption options)
        {
            _logger.LogInformation("Request received for bulk delete provider course locations for Ukprn:{ukprn} LarsCode:{larscode} DeleteOptions:{deleteOptions}", ukprn, larsCode, options);

            var command = new BulkDeleteProviderCourseLocationsCommand(ukprn, larsCode, options);
            var result = await _mediator.Send(command);

            _logger.LogInformation("Deleted {numberOfRecordsDeleted} provider course locations for Ukprn:{ukprn} LarsCode:{larscode}", result, ukprn, larsCode);

            return result > 0 ? StatusCode((int)HttpStatusCode.NoContent) : StatusCode((int)HttpStatusCode.NotFound);
        }
    }
}
