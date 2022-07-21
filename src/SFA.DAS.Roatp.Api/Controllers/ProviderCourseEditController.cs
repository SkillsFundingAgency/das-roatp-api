using System.Collections.Generic;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Api.Models;
using SFA.DAS.Roatp.Application.ProviderCourse;
using System.Threading.Tasks;
using Microsoft.AspNetCore.JsonPatch;

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



        // [Route("/providers/{ukprn}/courses/{larsCode}")]
        // [HttpPatch]
        // public async Task<IActionResult> Patch2([FromRoute] int ukprn, [FromRoute] int larsCode)
        // {
        //     _logger.LogInformation("Inner API: Request to patch course contact details for ukprn: {ukprn} larscode: {larscode} userid:{userid}", ukprn, larsCode, "xxx");
        //     // var command = (UpdateProviderCourseCommand)providerCourseEditModel;
        //     // command.Ukprn = ukprn;
        //     // command.LarsCode = larsCode;
        //     // await _mediator.Send(command);
        //     return NoContent();
        // }



        [Route("/providers/{ukprn}/courses/{larsCode}")]
        [HttpPatch]
        public async Task<IActionResult> Patch([FromRoute] int ukprn, [FromRoute] int larsCode, [FromBody] List<PatchOperation> request) //[FromBody] JsonPatchDocument<PatchProviderCourse> request)
        {
            var userId = "here"; // request.UserId;
            _logger.LogInformation("Inner API: Request to patch course contact details for ukprn: {ukprn} larscode: {larscode} userid:{userid}", ukprn, larsCode, userId);
            // var command = (UpdateProviderCourseCommand)providerCourseEditModel;
            // command.Ukprn = ukprn;
            // command.LarsCode = larsCode;
            // await _mediator.Send(command);
            return NoContent();
        }
    }
}
