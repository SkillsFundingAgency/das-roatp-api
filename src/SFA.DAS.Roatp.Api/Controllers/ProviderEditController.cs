using System.Threading.Tasks;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Api.Infrastructure;
using SFA.DAS.Roatp.Api.Models;
using SFA.DAS.Roatp.Application.Providers.Commands.CreateProvider;
using SFA.DAS.Roatp.Application.Providers.Commands.PatchProvider;
using SFA.DAS.Roatp.Domain.Models;

namespace SFA.DAS.Roatp.Api.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]

    [Route("/providers")]
    public class ProviderEditController : ActionResponseControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ProviderEditController> _logger;

        public ProviderEditController(IMediator mediator, ILogger<ProviderEditController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [Route("{ukprn}")]
        [HttpPatch]
        public async Task<IActionResult> PatchProvider([FromRoute] int ukprn, [FromBody] JsonPatchDocument<PatchProvider> request, [FromQuery] string userId, [FromQuery] string userDisplayName)
        {
            _logger.LogInformation("Inner API: Request to patch provider for ukprn: {ukprn}", ukprn);

            await _mediator.Send(new PatchProviderCommand
            {
                Ukprn = ukprn,
                UserId = userId,
                UserDisplayName = userDisplayName,
                Patch = request
            });

            return NoContent();
        }

        [HttpPost]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateProvider(ProviderAddModel providerAddModel, [FromQuery] string userId, [FromQuery] string userDisplayName)
        {
            _logger.LogInformation("Inner API: Received command to add provider: {ukprn}", providerAddModel.Ukprn);

            CreateProviderCommand command = providerAddModel;
            command.UserId = userId;
            command.UserDisplayName = userDisplayName;

            var response = await _mediator.Send(command);

            return GetPostResponse(response, $"/providers/{providerAddModel.Ukprn}");
        }
    }
}
