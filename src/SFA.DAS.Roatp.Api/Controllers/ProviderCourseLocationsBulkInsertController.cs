using System.Threading.Tasks;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Api.Infrastructure;
using SFA.DAS.Roatp.Application.ProviderCourseLocations.Commands.BulkInsert;
using static SFA.DAS.Roatp.Api.Infrastructure.Constants;

namespace SFA.DAS.Roatp.Api.Controllers
{
    [ApiController]
    [ApiVersion(ApiVersionNumber.One)]
    public class ProviderCourseLocationsBulkInsertController : ActionResponseControllerBase
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
        public async Task<IActionResult> BulkInsertProviderCourseLocations([FromRoute] int ukprn, BulkInsertProviderCourseLocationsCommand command)
        {
            _logger.LogInformation("Inner API: Request received to bulk insert provider course locations ukprn: {ukprn} larscode: {larscode} userid:{userid}", ukprn, command.LarsCode, command.UserId);
            command.Ukprn = ukprn;
            var response = await _mediator.Send(command);
            if (response.IsValidResponse)
                _logger.LogInformation("Inserted {numberOfRecordsInserted} provider course locations for Ukprn:{ukprn} LarsCode:{larscode}", response.Result, ukprn, command.LarsCode);

            return GetNoContentResponse(response);
        }
    }
}
