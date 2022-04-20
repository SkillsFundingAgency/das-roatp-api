using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Api.Models;
using SFA.DAS.Roatp.Api.Services;

namespace SFA.DAS.Roatp.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProviderCoursesController : ControllerBase
    {
        private readonly ILogger<ProviderCoursesController> _logger;
        private readonly IGetProviderCoursesService _getProviderCoursesService;

        public ProviderCoursesController(
            ILogger<ProviderCoursesController> logger,
            IGetProviderCoursesService getProviderCoursesService)
        {
            _logger = logger;
            _getProviderCoursesService = getProviderCoursesService;
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
            if (ukprn <= 0) return new BadRequestObjectResult("Invalid ukprn");

            var result = await _getProviderCoursesService.GetAllCourses(ukprn);

            if (!result.Any()) return new NotFoundObjectResult($"No data found for {ukprn}");

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
            if (ukprn <= 0 || larsCode <= 0) return new BadRequestObjectResult("Invalid ukprn or larscode.");

            var result = await _getProviderCoursesService.GetCourse(ukprn, larsCode);

            if (result == null) return new NotFoundObjectResult($"No data found for {ukprn} and {larsCode}");

            return new OkObjectResult(result);
        }
    }
}
