﻿using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Api.Infrastructure;
using SFA.DAS.Roatp.Api.Models;
using SFA.DAS.Roatp.Application.Locations.Commands.UpdateProviderLocationDetails;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Api.Controllers
{
    [ApiController]
    public class ProviderLocationEditController : ActionResponseControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ProviderLocationEditController> _logger;

        public ProviderLocationEditController(IMediator mediator, ILogger<ProviderLocationEditController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [Route("/providers/{ukprn}/locations/{id}")]
        [HttpPut]
        public async Task<IActionResult> Save([FromRoute]int ukprn, [FromRoute]Guid id, ProviderLocationEditModel providerLocationEditModel)
        {
            _logger.LogInformation("Inner API: Request to update provider location details for ukprn: {ukprn} id: {id} userid:{userid}", ukprn, id, providerLocationEditModel.UserId);
            var command = (UpdateProviderLocationDetailsCommand) providerLocationEditModel;
            command.Ukprn = ukprn;
            command.Id = id;
            var response = await _mediator.Send(command);

            return GetNoContentResponse(response);
        }
    }
}
