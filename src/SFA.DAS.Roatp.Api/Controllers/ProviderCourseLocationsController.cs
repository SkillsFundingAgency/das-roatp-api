using System.Collections.Generic;
using System.Threading.Tasks;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Api.Infrastructure;
using SFA.DAS.Roatp.Application.ProviderCourseLocations.Queries.GetProviderCourseLocations;
using static SFA.DAS.Roatp.Api.Infrastructure.Constants;

namespace SFA.DAS.Roatp.Api.Controllers;

[ApiController]
[ApiVersion(ApiVersionNumber.One)]
[Tags(EndpointTags.ProiderCourseLocations)]
[Route("/providers/{ukprn}/courses/{larsCode}/locations", Name = RouteNames.GetProviderCourseLocations)]
public class ProviderCourseLocationsController : ActionResponseControllerBase
{
    private readonly ILogger<ProviderCourseLocationsController> _logger;
    private readonly IMediator _mediator;

    public ProviderCourseLocationsController(ILogger<ProviderCourseLocationsController> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    [HttpGet]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(List<ProviderCourseLocationModel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProviderCourseLocations(int ukprn, string larsCode)
    {
        _logger.LogInformation("Request received to get all provider Course Locations for ukprn: {Ukprn}, larsCode : {LarsCode}", ukprn, larsCode);

        var response = await _mediator.Send(new GetProviderCourseLocationsQuery(ukprn, larsCode));

        if (response.IsValidResponse)
            _logger.LogInformation("Found {LocationCount} locations for ukprn: {Ukprn}, larsCode : {LarsCode}", response.Result.Count, ukprn, larsCode);

        return GetResponse(response);
    }
}
