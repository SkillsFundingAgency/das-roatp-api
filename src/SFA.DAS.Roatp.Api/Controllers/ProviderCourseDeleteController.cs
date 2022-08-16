using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Application.ProviderCourse.Commands.DeleteProviderCourse;

namespace SFA.DAS.Roatp.Api.Controllers
{
    [ApiController]
    public class ProviderCourseDeleteController : Controller
    {
        private readonly ILogger<ProviderCourseDeleteController> _logger;
        private readonly IMediator _mediator;

        public ProviderCourseDeleteController(ILogger<ProviderCourseDeleteController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [HttpDelete]
        [Route("/providers/{ukprn}/courses/{larsCode}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> DeleteProviderCourse([FromRoute] int ukprn, [FromRoute] int larsCode, [FromQuery] string userId)
        {
            _logger.LogInformation("Inner API: Request received to delete provider course ukprn: {ukprn} larscode: {larscode} userid:{userid}", ukprn, larsCode, userId);
            
            var command = new DeleteProviderCourseCommand(ukprn, larsCode,  userId);
            await _mediator.Send(command);

            _logger.LogInformation("Deleted provider course for Ukprn:{ukprn} LarsCode:{larscode}", ukprn, larsCode);

            return NoContent();
        }
    }
}
