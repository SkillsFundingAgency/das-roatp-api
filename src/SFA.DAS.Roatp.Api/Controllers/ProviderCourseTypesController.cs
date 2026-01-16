using System.Collections.Generic;
using System.Threading.Tasks;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Api.Infrastructure;
using SFA.DAS.Roatp.Application.ProviderCourseTypes.Queries.GetProviderCourseTypes;

namespace SFA.DAS.Roatp.Api.Controllers;

[ApiVersion("1.0")]

[Route("/providers/{ukprn}/course-types", Name = RouteNames.GetProviderCourseTypes)]
public class ProviderCourseTypesController(IMediator _mediator, ILogger<ProviderCourseLocationsController> _logger) : ActionResponseControllerBase
{
    [HttpGet]
    [Produces("application/json")]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(List<ProviderCourseTypeModel>), 200)]
    public async Task<IActionResult> GetProviderCourseTypes(int ukprn)
    {
        _logger.LogInformation("Request received to get all provider Course Types for ukprn: {Ukprn}", ukprn);

        var response = await _mediator.Send(new GetProviderCourseTypesQuery(ukprn));

        if (response.IsValidResponse)
            _logger.LogInformation("Found {Count} courseTypes for ukprn: {Ukprn}", response.Result.Count, ukprn);

        return GetResponse(response);
    }
}
