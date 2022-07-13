using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Application.Locations.Queries;
using SFA.DAS.Roatp.Application.ProviderCourse.Queries;
using ProviderCourseModel = SFA.DAS.Roatp.Api.Models.ProviderCourseModel;

namespace SFA.DAS.Roatp.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProviderCoursesController : ControllerBase
    {
        private readonly ILogger<ProviderCoursesController> _logger;
        private readonly IMediator _mediator;

        public ProviderCoursesController(
            ILogger<ProviderCoursesController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        /// <summary>
        /// Gets all the courses for the given provider
        /// </summary>
        /// <param name="ukprn"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("/providers/{ukprn}/courses")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(List<ProviderCourseModel>), 200)]
        public async Task<ActionResult<List<ProviderCourseModel>>> GetAllCourses(int ukprn)
        {
            var allCoursesResult = await _mediator.Send(new ProviderAllCoursesQuery(ukprn));
            var result = allCoursesResult.Courses;
            _logger.LogInformation("Courses data found for {ukprn}", ukprn);
            return new OkObjectResult(result);
        }

        /// <summary>
        /// Gets all the available delivery models for the given provider and standard
        /// </summary>
        /// <param name="ukprn"></param>
        /// <param name="larsCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("/providers/{ukprn}/courses/{larsCode}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProviderCourseModel), 200)]
        public async Task<ActionResult<ProviderCourseModel>> GetCourse(int ukprn, int larsCode)
        {
            var courseResult = await _mediator.Send(new ProviderCourseQuery(ukprn, larsCode));
            var result = courseResult.Course;
            _logger.LogInformation("Course data found for {ukprn} and {larsCode}", ukprn, larsCode);
            return new OkObjectResult(result);
        }
    }
}
