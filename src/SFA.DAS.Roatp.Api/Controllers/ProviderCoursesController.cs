using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Roatp.Api.Infrastructure;
using SFA.DAS.Roatp.Application.ProviderCourse.Queries.GetAllProviderCourses;
using SFA.DAS.Roatp.Application.ProviderCourse.Queries.GetProviderCourse;

namespace SFA.DAS.Roatp.Api.Controllers
{
    [ApiController]
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
        /// <returns></returns>
        [HttpGet]
        [Route("/providers/{ukprn}/courses")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(List<ProviderCourseModel>), 200)]
        public async Task<IActionResult> GetAllCourses(int ukprn)
        {
            var response = await _mediator.Send(new GetAllProviderCoursesQuery(ukprn));
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
        public async Task<IActionResult> GetCourse(int ukprn, int larsCode)
        {
            var response = await _mediator.Send(new GetProviderCourseQuery(ukprn, larsCode));
            return GetResponse(response);
        }
    }
}
