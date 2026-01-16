using System.Threading.Tasks;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Api.Infrastructure;
using SFA.DAS.Roatp.Api.Models;
using SFA.DAS.Roatp.Application.ProviderCourseLocations.Commands.AddNationalLocation;
using SFA.DAS.Roatp.Application.ProviderCourseLocations.Commands.AddProviderCourseLocation;

namespace SFA.DAS.Roatp.Api.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    public class ProviderCourseLocationsEditController : ActionResponseControllerBase
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
        public async Task<IActionResult> AddNationalLocationToProviderCourseLocations([FromRoute] int ukprn, [FromRoute] string larsCode, AddNationalLocationToProviderCourseLocationsModel model)
        {
            _logger.LogInformation("Inner API: Request to create national location received for ukprn: {ukprn} larsCode: {larscode}", ukprn, larsCode);
            var command = new AddNationalLocationToProviderCourseLocationsCommand(ukprn, larsCode, model.UserId, model.UserDisplayName);
            var response = await _mediator.Send(command);

            return GetPostResponse(response, $"/providers/{ukprn}/courses/{larsCode}/locations");
        }

        [HttpPost]
        [Route("providers/{ukprn}/courses/{larsCode}/locations")]
        public async Task<IActionResult> CreateProviderCourseLocation([FromRoute] int ukprn, [FromRoute] string larsCode, AddProviderCourseLocationModel model)
        {
            _logger.LogInformation("Inner API: Request to create provider course location received for ukprn: {ukprn} larsCode: {larscode} locationNavigationId : {locationNavigationId}", ukprn, larsCode, model.LocationNavigationId);
            var command = new AddProviderCourseLocationCommand(ukprn, larsCode, model.UserId, model.UserDisplayName, model.LocationNavigationId, model.HasDayReleaseDeliveryOption, model.HasBlockReleaseDeliveryOption);
            var response = await _mediator.Send(command);

            return GetPostResponse(response, $"/providers/{ukprn}/courses/{larsCode}/locations");
        }
    }
}
