using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Api.Models;
using SFA.DAS.Roatp.Application.ProviderCourse.Commands.UpdateProviderCourse;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Api.Controllers
{
    [ApiController]
    public class ProviderCourseEditController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ProviderCourseEditController> _logger;

        public ProviderCourseEditController(IMediator mediator, ILogger<ProviderCourseEditController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [Route("/providers/{ukprn}/courses/{larsCode}")]
        [HttpPut]
        public async Task<IActionResult> Save([FromRoute]int ukprn, [FromRoute]int larsCode, ProviderCourseEditModel providerCourseEditModel)
        {
            _logger.LogInformation("Inner API: Request to update course contact details for ukprn: {ukprn} larscode: {larscode} userid:{userid}", ukprn, larsCode, providerCourseEditModel.UserId);
            var command = (UpdateProviderCourseCommand) providerCourseEditModel;
            command.Ukprn = ukprn;
            command.LarsCode = larsCode;
            await _mediator.Send(command);
            return NoContent();
        }
    }
}
