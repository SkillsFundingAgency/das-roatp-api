using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Application.ProviderCourse.Queries.GetProvidersCountForCourse;
using System.Threading.Tasks;
using SFA.DAS.Roatp.Api.Infrastructure;
using SFA.DAS.Roatp.Application.Courses.Queries.GetProviderDetailsForCourse;

namespace SFA.DAS.Roatp.Api.Controllers.ExternalReadControllers
{
    [ApiController]
    [Route("/api/[controller]/")]
    public class CoursesController : ActionResponseControllerBase
    {
        private readonly ILogger<CoursesController> _logger;
        private readonly IMediator _mediator;

        public CoursesController(IMediator mediator, ILogger<CoursesController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet]
        [Route("{larsCode}/providers/count")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(GetProvidersCountForCourseQueryResult), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetProvidersCountForCourse(int larsCode)
        {
            _logger.LogInformation("Received request to get total providers associated with Larscode:{larscode}", larsCode);
            var response = await _mediator.Send(new GetProvidersCountForCourseQuery(larsCode));
            if (response.IsValidResponse)
                _logger.LogInformation("Found {providerCount} providers that are associated with Larscode:{larscode}", response.Result.ProvidersCount, larsCode);
            return GetResponse(response);
        }

        [HttpGet]
        [Route("{larsCode}/providers/{ukprn}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(GetProviderDetailsForCourseQueryResult), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetProviderDetailsForCourse(int larsCode, int ukprn, decimal? lat = null, decimal? lon = null)
        {
            _logger.LogInformation("Received request to get provider details for Ukprn: {ukprn}, LarsCode: {larscode},  Latitude: {latitude}, Long: {longitude}", ukprn, larsCode, lat, lon);
            var response = await _mediator.Send(new GetProviderDetailsForCourseQuery(larsCode, ukprn, lat, lon));
            return GetResponse(response);
        }

        [HttpGet]
        [Route("{larsCode}/providers")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(GetProvidersForCourseQueryResult), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetProvidersForCourse(int larsCode, decimal? lat = null, decimal? lon = null)
        {
            _logger.LogInformation("Received request to get list of provider details for LarsCode: {larscode},  Latitude: {latitude}, Longitude: {longitude}", larsCode, lat, lon);
            var response = await _mediator.Send(new GetProvidersForCourseQuery(larsCode, lat, lon));
            return GetResponse(response);
        }
    }
}
