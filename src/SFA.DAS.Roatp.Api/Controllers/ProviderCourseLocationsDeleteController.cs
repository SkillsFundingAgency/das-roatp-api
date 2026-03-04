using System;
using System.Threading.Tasks;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Api.Infrastructure;
using SFA.DAS.Roatp.Application.ProviderCourseLocations.Commands.BulkDelete;
using SFA.DAS.Roatp.Application.ProviderCourseLocations.Commands.Delete;
using static SFA.DAS.Roatp.Api.Infrastructure.Constants;

namespace SFA.DAS.Roatp.Api.Controllers;

[ApiController]
[ApiVersion(ApiVersionNumber.One)]
[Tags(EndpointTags.ProiderCourseLocations)]
[Route("/providers/{ukprn}/courses/{larsCode}/locations")]
public class ProviderCourseLocationsDeleteController : ActionResponseControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<ProviderCourseLocationsDeleteController> _logger;

    public ProviderCourseLocationsDeleteController(IMediator mediator, ILogger<ProviderCourseLocationsDeleteController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> BulkDeleteProviderCourseLocations([FromRoute] int ukprn, [FromRoute] string larsCode, [FromQuery] DeleteProviderCourseLocationOption options, [FromQuery] string userId, [FromQuery] string userDisplayName)
    {
        _logger.LogInformation("Inner API: Request received for bulk delete provider course locations for Ukprn:{Ukprn} LarsCode:{Larscode} DeleteOptions:{DeleteOptions}", ukprn, larsCode, options);

        var command = new BulkDeleteProviderCourseLocationsCommand(ukprn, larsCode, options, userId, userDisplayName);
        var response = await _mediator.Send(command);

        if (response.IsValidResponse)
            _logger.LogInformation("Deleted {NumberOfRecordsDeleted} provider course locations for Ukprn:{Ukprn} LarsCode:{Larscode}", response.Result, ukprn, larsCode);

        return GetNoContentResponse(response);
    }

    [HttpDelete]
    [Route("{id}")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteProviderCourseLocation([FromRoute] int ukprn, [FromRoute] string larsCode, [FromRoute] Guid id, [FromQuery] string userId, [FromQuery] string userDisplayName)
    {
        _logger.LogInformation("Inner API: Request received for delete provider course location for Ukprn:{Ukprn} LarsCode:{Larscode} ProviderCourseLocationId:{ProviderCourseLocaitonId}", ukprn, larsCode, id);

        var command = new DeleteProviderCourseLocationCommand(ukprn, larsCode, id, userId, userDisplayName);
        var response = await _mediator.Send(command);

        if (response.IsValidResponse)
            _logger.LogInformation("Deleted provider course locations for Ukprn:{Ukprn} LarsCode:{LarsCode} ProviderCourseLocationId:{ProviderCourseLocationId}", ukprn, larsCode, id);

        return GetNoContentResponse(response);
    }
}
