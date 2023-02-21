using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Api.Infrastructure;
using SFA.DAS.Roatp.Application.ProviderCourseLocations.Queries.GetProviderCourseLocations;

namespace SFA.DAS.Roatp.Api.Controllers
{
    [ApiController]
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
        [Route("/providers/{ukprn}/courses/{larsCode}/locations", Name = RouteNames.GetProviderCourseLocations)]
        [Produces("application/json")]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(List<ProviderCourseLocationModel>), 200)]
        public async Task<IActionResult> GetProviderCourseLocations(int ukprn, int larsCode)
        {
            _logger.LogInformation("Request received to get all provider Course Locations for ukprn: {ukprn}, larsCode : {larsCode}", ukprn, larsCode);

            var response = await _mediator.Send(new GetProviderCourseLocationsQuery(ukprn, larsCode));

            if (response.IsValidResponse)
                _logger.LogInformation("Found {locationCount} locations for ukprn: {ukprn}, larsCode : {larsCode}", response.Result.Count, ukprn, larsCode);

            return GetResponse(response);
        }
    }
}
