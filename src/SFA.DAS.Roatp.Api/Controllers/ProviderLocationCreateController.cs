﻿using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Application.Locations.Commands.CreateLocation;
using System.Threading.Tasks;
using SFA.DAS.Roatp.Api.Infrastructure;
using SFA.DAS.Roatp.Application.Common;

namespace SFA.DAS.Roatp.Api.Controllers
{
    [ApiController]
    public class ProviderLocationCreateController : ActionResponseControllerBase
    {
        private readonly ILogger<ProviderLocationCreateController> _logger;
        private readonly IMediator _mediator;

        public ProviderLocationCreateController(IMediator mediator, ILogger<ProviderLocationCreateController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpPost]
        [Route("/providers/{ukprn}/locations")]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateLocation([FromRoute] int ukprn, CreateProviderLocationCommand command)
        {
            _logger.LogInformation("Request to save Provider Location {locationName} for Ukprn {ukprn}", command.LocationName, ukprn);

            if (ukprn != command.Ukprn)
            {
                return BadRequest($"Route ukprn: {ukprn} is different than request ukprn: {command.Ukprn}");
            }

            var response = await _mediator.Send(command);

            return GetPostResponse(response, $"/providers/{ukprn}/locations");
        }
    }
}
