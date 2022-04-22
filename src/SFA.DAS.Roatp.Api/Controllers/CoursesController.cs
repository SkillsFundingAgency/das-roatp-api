using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Api.Models;

namespace SFA.DAS.Roatp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoursesController : ControllerBase
    {
        private readonly ILogger<CoursesController> _logger;


        [HttpPost]
        [Route("/UpdateCourses")]
        //[Produces("application/json")]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(List<ProviderCourseModel>), 200)]
        public async Task<ActionResult<bool>> UpdateCourses(CoursesRequest courseRequest)
        {
            var coursesToProcess = courseRequest.Courses;
            return true;
        }
    }
}
