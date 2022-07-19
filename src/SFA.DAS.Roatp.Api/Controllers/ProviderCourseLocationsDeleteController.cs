﻿using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Application.ProviderCourseLocations.Commands.BulkDelete;
using SFA.DAS.Roatp.Application.ProviderCourseLocations.Commands.Delete;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Api.Controllers
{
    [ApiController]
    public class ProviderCourseLocationsDeleteController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ProviderCourseLocationsDeleteController> _logger;

        public ProviderCourseLocationsDeleteController(IMediator mediator, ILogger<ProviderCourseLocationsDeleteController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpDelete]
        [Route("/providers/{ukprn}/courses/{larsCode}/locations")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> BulkDeleteProviderCourseLocations([FromRoute] int ukprn, [FromRoute] int larsCode, [FromQuery] DeleteProviderCourseLocationOption options, [FromQuery] string userId )
        {
            _logger.LogInformation("Inner API: Request received for bulk delete provider course locations for Ukprn:{ukprn} LarsCode:{larscode} DeleteOptions:{deleteOptions}", ukprn, larsCode, options);

            var command = new BulkDeleteProviderCourseLocationsCommand(ukprn, larsCode, options, userId);
            var result = await _mediator.Send(command);

            _logger.LogInformation("Deleted {numberOfRecordsDeleted} provider course locations for Ukprn:{ukprn} LarsCode:{larscode}", result, ukprn, larsCode);

            return NoContent();
        }

        [HttpDelete]
        [Route("/providers/{ukprn}/courses/{larsCode}/location/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> DeleteProviderCourseLocation([FromRoute] int ukprn, [FromRoute] int larsCode, [FromRoute] int id, [FromQuery] string userId)
        {
            _logger.LogInformation("Inner API: Request received for delete provider course location for Ukprn:{ukprn} LarsCode:{larscode} ProviderCourseLocationId:{id}", ukprn, larsCode, id);

            var command = new DeleteProviderCourseLocationCommand(ukprn, larsCode, id, userId);
            await _mediator.Send(command);

            _logger.LogInformation("Deleted provider course locations for Ukprn:{ukprn} LarsCode:{larscode} ProviderCourseLocationId:{id}", ukprn, larsCode, id);

            return NoContent();
        }
    }
}
