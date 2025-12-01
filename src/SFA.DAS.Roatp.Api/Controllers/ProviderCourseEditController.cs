using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Api.Infrastructure;
using SFA.DAS.Roatp.Api.Models;
using SFA.DAS.Roatp.Application.ProviderCourse.Commands.CreateProviderCourse;
using SFA.DAS.Roatp.Application.ProviderCourse.Commands.PatchProviderCourse;
using SFA.DAS.Roatp.Domain.Models;

namespace SFA.DAS.Roatp.Api.Controllers
{
    [ApiController]
    public class ProviderCourseEditController : ActionResponseControllerBase
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
        public async Task<IActionResult> PatchProviderCourse([FromRoute] int ukprn, [FromRoute] string larsCode, [FromBody] JsonPatchDocument<PatchProviderCourse> request, [FromQuery] string userId, [FromQuery] string userDisplayName)
        {
            _logger.LogInformation("Inner API: Request to patch course contact details for ukprn: {ukprn} larscode: {larscode}", ukprn, larsCode);

            var response = await _mediator.Send(new PatchProviderCourseCommand
            {
                Ukprn = ukprn,
                LarsCode = larsCode,
                UserId = userId,
                UserDisplayName = userDisplayName,
                Patch = request
            });

            return GetNoContentResponse(response);
        }

        [Route("/providers/{ukprn}/courses/{larsCode}")]
        [HttpPost]
        public async Task<IActionResult> CreateProviderCourse([FromRoute] int ukprn, [FromRoute] string larsCode, ProviderCourseAddModel providerCourseAddModel, [FromQuery] string userId, [FromQuery] string userDisplayName)
        {
            _logger.LogInformation("Inner API: Received command to add course: {larscode} to provider: {ukprn}", larsCode, ukprn);

            CreateProviderCourseCommand command = providerCourseAddModel;
            command.Ukprn = ukprn;
            command.LarsCode = larsCode;
            command.UserId = userId;
            command.UserDisplayName = userDisplayName;

            var response = await _mediator.Send(command);

            return GetPostResponse(response, $"/providers/{ukprn}/courses");
        }
    }
}
