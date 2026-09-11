using System.Threading;
using System.Threading.Tasks;
using Asp.Versioning;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Api.Infrastructure;
using SFA.DAS.Roatp.Api.Models;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Application.ProviderAllowedCourses.Commands.CreateProviderAllowedCourse;
using SFA.DAS.Roatp.Application.ProviderAllowedCourses.Commands.PatchProviderAllowedCourse;
using SFA.DAS.Roatp.Application.ProviderAllowedCourses.Queries.GetProviderAllowedCourses;
using SFA.DAS.Roatp.Domain.Models;
using static SFA.DAS.Roatp.Api.Infrastructure.Constants;

namespace SFA.DAS.Roatp.Api.Controllers;

[ApiController]
[ApiVersion(ApiVersionNumber.One)]
[Tags(EndpointTags.ProviderAllowedCourses)]
[Route("providers/{ukprn}/allowed-courses")]
public class ProviderAllowedCoursesController(IMediator _mediator, ILogger<ProviderAllowedCoursesController> _logger, IValidator<IUkprnAndLarsCodeValidator> _validator) : ActionResponseControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(GetProviderAllowedCoursesQueryResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllowedCourses([FromRoute] int ukprn, [FromQuery] CourseType? courseType = null, CancellationToken cancellationToken = default)
    {
        GetProviderAllowedCoursesQuery query = new(ukprn, courseType);
        GetProviderAllowedCoursesQueryResult result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpPost("{larsCode}")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> AddProviderAllowedCourse([FromRoute] int ukprn, [FromRoute] string larsCode, [FromBody] CreateProviderAllowedCourseModel request)
    {
        _logger.LogInformation("Request to add provider allowed course for Ukprn {Ukprn} and LarsCode {LarsCode}", ukprn, larsCode);

        var model = new UkrpnAndLarsCodeModel
        {
            Ukprn = ukprn,
            LarsCode = larsCode
        };

        var result = await _validator.ValidateAsync(model);

        if (!result.IsValid)
        {
            return NotFound(FormatErrors(result.Errors));
        }

        CreateProviderAllowedCourseCommand command = new()
        {
            Ukprn = ukprn,
            LarsCode = larsCode.ToUpper().Trim(),
            UserId = request.UserId,
            UserDisplayName = request.UserDisplayName,
            LastDateStarts = request.LastDateStarts,
            IsStartRestricted = request.IsStartRestricted
        };

        var response = await _mediator.Send(command);

        return GetNoContentResponse(response);
    }

    [HttpPatch("{larsCode}")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> PatchProviderAllowedCourse([FromRoute] int ukprn, [FromRoute] string larsCode, [FromBody] JsonPatchDocument<PatchProviderAllowedCourseModel> request, [FromQuery] string userId, [FromQuery] string userDisplayName)
    {
        _logger.LogInformation("Request to patch provider allowedCourse for Ukprn {Ukprn} and LarsCode {LarsCode}", ukprn, larsCode);

        var model = new UkrpnAndLarsCodeModel
        {
            Ukprn = ukprn,
            LarsCode = larsCode
        };

        var result = await _validator.ValidateAsync(model);

        if (!result.IsValid)
        {
            return NotFound(FormatErrors(result.Errors));
        }

        PatchProviderAllowedCourseCommand command = new()
        {
            Ukprn = ukprn,
            LarsCode = larsCode.ToUpper().Trim(),
            UserId = userId,
            UserDisplayName = userDisplayName,
            PatchDoc = request
        };

        var response = await _mediator.Send(command);

        return GetNoContentResponse(response);
    }
}
