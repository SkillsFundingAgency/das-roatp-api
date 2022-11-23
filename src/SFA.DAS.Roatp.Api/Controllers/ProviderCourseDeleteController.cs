﻿using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Application.Locations.Commands.BulkDelete;
using SFA.DAS.Roatp.Application.ProviderCourse.Commands.DeleteProviderCourse;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Api.Controllers
{
    [ApiController]
    public class ProviderCourseDeleteController : Controller
    {
        private readonly ILogger<ProviderCourseDeleteController> _logger;
        private readonly IMediator _mediator;

        public ProviderCourseDeleteController(ILogger<ProviderCourseDeleteController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [HttpDelete]
        [Route("/providers/{ukprn}/courses/{larsCode}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> DeleteProviderCourse([FromRoute] int ukprn, [FromRoute] int larsCode, [FromQuery] string userId, [FromQuery] string UserDisplayName)
        {
            _logger.LogInformation("Inner API: Request received to delete provider course ukprn: {ukprn} larscode: {larscode} userid:{userid}", ukprn, larsCode, userId);

            var command = new DeleteProviderCourseCommand(ukprn, larsCode,  userId, UserDisplayName);
            await _mediator.Send(command);
            _logger.LogInformation("Deleted provider course for Ukprn:{ukprn} LarsCode:{larscode}", ukprn, larsCode);

            var bulkDeleteProviderLocationsCommand = new BulkDeleteProviderLocationsCommand(ukprn, userId, UserDisplayName);
            var result = await _mediator.Send(bulkDeleteProviderLocationsCommand);

            _logger.LogInformation("Deleted {numberOfRecordsDeleted} provider locations for Ukprn:{ukprn}", result, ukprn);
            return NoContent();
        }
    }
}
