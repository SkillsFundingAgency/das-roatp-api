using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Api.Models;
using SFA.DAS.Roatp.Application.ProviderCourseLocations.Commands.AddNationalLocation;
using SFA.DAS.Roatp.Application.ProviderCourseLocations.Commands.AddProviderCourseLocation;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Api.Controllers
{
    [ApiController]
    public class ProviderCourseLocationsEditController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ProviderCourseLocationsEditController> _logger;

        public ProviderCourseLocationsEditController(IMediator mediator, ILogger<ProviderCourseLocationsEditController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpPost]
        [Route("/providers/{ukprn}/courses/{larsCode}/locations/national")]
        public async Task<IActionResult> AddNationalLocationToProviderCourseLocations([FromRoute] int ukprn, [FromRoute] int larsCode, AddNationalLocationToProviderCourseLocationsModel model)
        {
            _logger.LogInformation("Inner API: Request to create national location received for ukprn: {ukprn} larsCode: {larscode}", ukprn, larsCode);
            var command = new AddNationalLocationToProviderCourseLocationsCommand(ukprn, larsCode, model.UserId);
            var providerCourseLocation = await _mediator.Send(command);
            return CreatedAtRoute(RouteNames.GetProviderCourseLocations, new { ukprn, larsCode }, providerCourseLocation.Id);
        }

        [HttpPost]
        [Route("providers/{ukprn}/courses/{larsCode}/locations")]
        public async Task<IActionResult> CreateProviderCourseLocation([FromRoute] int ukprn, [FromRoute] int larsCode, AddProviderCourseLocationModel model)
        {
            _logger.LogInformation("Inner API: Request to create provider course location received for ukprn: {ukprn} larsCode: {larscode} locationNavigationId : {locationNavigationId}", ukprn, larsCode, model.LocationNavigationId);
            var command = new AddProviderCourseLocationCommand(ukprn, larsCode, model.UserId, model.LocationNavigationId, model.HasDayReleaseDeliveryOption, model.HasBlockReleaseDeliveryOption);
            var providerCourseLocationId = await _mediator.Send(command);
            return CreatedAtRoute(RouteNames.GetProviderCourseLocations, new { ukprn, larsCode }, providerCourseLocationId);
        }
    }
}
