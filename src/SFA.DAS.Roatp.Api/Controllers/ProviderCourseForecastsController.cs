using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Roatp.Api.Infrastructure;
using SFA.DAS.Roatp.Application.Mediatr.Responses;
using SFA.DAS.Roatp.Application.ProviderCourseForecasts.Commands.UpsertProviderCourseForecasts;
using SFA.DAS.Roatp.Application.ProviderCourseForecasts.Queries.GetProviderCourseForecasts;
using static SFA.DAS.Roatp.Api.Infrastructure.Constants;

namespace SFA.DAS.Roatp.Api.Controllers;

[ApiController]
[ApiVersion(ApiVersionNumber.One)]
[Tags(EndpointTags.ProviderCourses)]
[Route("providers/{ukprn}/courses/{larsCode}/forecasts")]
public class ProviderCourseForecastsController(IMediator _mediator) : ActionResponseControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(GetProviderCourseForecastsQueryResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(List<ValidationError>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProviderCourseForecasts(int ukprn, string larsCode, CancellationToken cancellationToken)
    {
        ValidatedResponse<GetProviderCourseForecastsQueryResult> result = await _mediator.Send(new GetProviderCourseForecastsQuery(ukprn, larsCode), cancellationToken);
        return GetResponse(result);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(List<ValidationError>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpsertProviderCourseForecasts([FromRoute] int ukprn, [FromRoute] string larsCode, [FromBody] IEnumerable<UpsertProviderCourseForecastModel> forecasts, CancellationToken cancellationToken)
    {
        ValidatedResponse response = await _mediator.Send(new UpsertProviderCourseForecastsCommand(ukprn, larsCode, forecasts), cancellationToken);

        if (response.IsValidResponse) return NoContent();

        return new BadRequestObjectResult(FormatErrors(response.Errors));
    }
}
