using System.Collections.Generic;
using System.Threading.Tasks;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Roatp.Api.Infrastructure;
using SFA.DAS.Roatp.Application.ProviderCourse.Queries.GetAllProviderCourses;
using SFA.DAS.Roatp.Application.ProviderCourse.Queries.GetProviderCourse;
using SFA.DAS.Roatp.Domain.Models;
using static SFA.DAS.Roatp.Api.Infrastructure.Constants;

namespace SFA.DAS.Roatp.Api.Controllers
{
    [ApiController]
    [ApiVersion(ApiVersionNumber.One)]
    [Route("[controller]")]
    public class ProviderCoursesController : ActionResponseControllerBase

    {
        private readonly IMediator _mediator;

        public ProviderCoursesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Gets all the courses for the given provider
        /// </summary>
        /// <param name="ukprn"></param>
        /// <param name="excludeCoursesWithoutLocation"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("/providers/{ukprn}/courses")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(List<ProviderCourseModel>), 200)]
        public async Task<IActionResult> GetAllCourses(int ukprn, [FromQuery] bool excludeCoursesWithoutLocation = true, [FromQuery] CourseType? courseType = null)
        {
            var response = await _mediator.Send(new GetAllProviderCoursesQuery(ukprn, excludeCoursesWithoutLocation, courseType));
            return GetResponse(response);
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
        public async Task<IActionResult> GetCourse(int ukprn, string larsCode)
        {
            var response = await _mediator.Send(new GetProviderCourseQuery(ukprn, larsCode));
            return GetResponse(response);
        }
    }
}
