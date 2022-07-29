using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Application.ProviderCourse;
using System.Threading.Tasks;
using Microsoft.AspNetCore.JsonPatch;
using SFA.DAS.Roatp.Domain.Models;

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
        [HttpPatch] 
        public async Task<IActionResult> PatchProviderCourse([FromRoute] int ukprn, [FromRoute] int larsCode, [FromBody] JsonPatchDocument<PatchProviderCourse> request)
        {
            _logger.LogInformation("Inner API: Request to patch course contact details for ukprn: {ukprn} larscode: {larscode}", ukprn, larsCode);

            await _mediator.Send(new PatchProviderCourseCommand
            {
                Ukprn = ukprn,
                LarsCode = larsCode,
                Patch = request
            });
            
            return NoContent();
        }
    }
}
