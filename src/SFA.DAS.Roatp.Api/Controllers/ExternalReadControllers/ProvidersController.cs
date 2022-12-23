using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Application.ProviderCourse.Queries.GetAllProviderCourses;
using SFA.DAS.Roatp.Application.Providers.Queries.GetProviders;
using SFA.DAS.Roatp.Application.Providers.Queries.GetProviderSummary;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.Roatp.Application.ProviderCourse.Queries.GetProviderCourse;

namespace SFA.DAS.Roatp.Api.Controllers.ExternalReadControllers
{
    [ApiController]
    [Route("/api/[controller]/")]
    public class ProvidersController : ControllerBase
    {
        private readonly ILogger<ProvidersController> _logger;
        private readonly IMediator _mediator;

        public ProvidersController(ILogger<ProvidersController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [HttpGet]
        [Route("")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(List<ProviderSummary>), 200)]
        public async Task<IActionResult> GetProviders()
        {
            var providerResult = await _mediator.Send(new GetProvidersQuery());
            _logger.LogInformation("Providers summary data found");
            return new OkObjectResult(providerResult);
        }

        [HttpGet]
        [Route("{ukprn}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ProviderSummary), 200)]
        public async Task<IActionResult> GetProvider(int ukprn)
        {
            var providerResult = await _mediator.Send(new GetProviderSummaryQuery(ukprn));
            _logger.LogInformation("Provider summary data found for {ukprn}:", ukprn);
            return new OkObjectResult(providerResult);
        }

        /// <summary>
        /// Gets all the courses for the given provider
        /// </summary>
        /// <param name="ukprn"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{ukprn}/courses")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(List<ProviderCourseModel>), 200)]
        public async Task<ActionResult<List<ProviderCourseModel>>> GetAllProviderCourses(int ukprn)
        {
            var allCoursesResult = await _mediator.Send(new GetAllProviderCoursesQuery(ukprn));
            var result = allCoursesResult.Courses;
            _logger.LogInformation("Courses data found for {ukprn}", ukprn);
            return new OkObjectResult(result);
        }

        [HttpGet]
        [Route("{ukprn}/courses/{larsCode}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProviderCourseModel), 200)]
        public async Task<ActionResult<ProviderCourseModel>> GetProviderCourse(int ukprn, int larsCode)
        {
            var courseResult = await _mediator.Send(new GetProviderCourseQuery(ukprn, larsCode));
            _logger.LogInformation("Course data found for {ukprn} and {larsCode}", ukprn, larsCode);
            return new OkObjectResult(courseResult.Course);
        }
    }
}
