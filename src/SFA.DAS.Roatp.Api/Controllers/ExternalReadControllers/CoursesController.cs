using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Application.ProviderCourse.Queries.GetProvidersCountForCourse;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Api.Controllers.ExternalReadControllers
{
    [ApiController]
    [Route("/api/[controller]/")]
    public class CoursesController : ControllerBase
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
        public async Task<ActionResult<GetProvidersCountForCourseQueryResult>> GetProvidersCountForCourse(int larsCode)
        {
            _logger.LogInformation("Received request to get total providers associated with Larscode:{larscode}", larsCode);
            var response = await _mediator.Send(new GetProvidersCountForCourseQuery(larsCode));
            _logger.LogInformation("Found {providerCount} providers that are associated with Larscode:{larscode}", response.ProvidersCount, larsCode);
            return Ok(response);
        }
    }
}
