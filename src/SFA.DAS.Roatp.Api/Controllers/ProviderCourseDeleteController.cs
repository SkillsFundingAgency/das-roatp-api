using System.Threading.Tasks;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Api.Infrastructure;
using SFA.DAS.Roatp.Application.Locations.Commands.BulkDelete;
using SFA.DAS.Roatp.Application.ProviderCourse.Commands.DeleteProviderCourse;
using static SFA.DAS.Roatp.Api.Infrastructure.Constants;

namespace SFA.DAS.Roatp.Api.Controllers
{
    [ApiController]
    [ApiVersion(ApiVersionNumber.One)]
    public class ProviderCourseDeleteController : ActionResponseControllerBase
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
        public async Task<IActionResult> DeleteProviderCourse([FromRoute] int ukprn, [FromRoute] string larsCode, [FromQuery] string userId, [FromQuery] string UserDisplayName)
        {
            _logger.LogInformation("Inner API: Request received to delete provider course ukprn: {ukprn} larscode: {larscode} userid:{userid}", ukprn, larsCode, userId);

            var command = new DeleteProviderCourseCommand(ukprn, larsCode, userId, UserDisplayName);
            await _mediator.Send(command);
            _logger.LogInformation("Deleted provider course for Ukprn:{ukprn} LarsCode:{larscode}", ukprn, larsCode);

            var bulkDeleteProviderLocationsCommand = new BulkDeleteProviderLocationsCommand(ukprn, userId, UserDisplayName);
            var response = await _mediator.Send(bulkDeleteProviderLocationsCommand);

            if (response.IsValidResponse)
                _logger.LogInformation("Deleted {numberOfRecordsDeleted} provider locations for Ukprn:{ukprn}", response.Result, ukprn);

            return GetNoContentResponse(response);
        }
    }
}
