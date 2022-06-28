﻿using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Api.Models;
using SFA.DAS.Roatp.Application.Locations.Commands.BulkInsert;
using SFA.DAS.Roatp.Application.Locations.Queries;
using SFA.DAS.Roatp.Application.ProviderCourseLocations.Commands.BulkInsert;

namespace SFA.DAS.Roatp.Api.Controllers
{
    [ApiController]
    [Route("[controller]/")]
    public class ProviderCourseLocationsBulkInsertController : Controller
    {
        private readonly ILogger<ProviderCourseLocationsBulkInsertController> _logger;
        private readonly IMediator _mediator;

        public ProviderCourseLocationsBulkInsertController(ILogger<ProviderCourseLocationsBulkInsertController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [HttpPost]
        [Route("/providers/{ukprn}/courses/{larsCode}/locations/bulk")]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status204NoContent)]
        public async Task<IActionResult> BulkInsertProviderCourseLocations([FromRoute] int ukprn, ProviderCourseLocationsInsertModel providerLocationsInsertModel)
        {
            _logger.LogInformation("Inner API: Request received to bulk insert locations ukprn: {ukprn} larscode: {larscode} userid:{userid}", ukprn, providerLocationsInsertModel.LarsCode, providerLocationsInsertModel.UserId);
            var command = (BulkInsertProviderCourseLocationsCommand)providerLocationsInsertModel;
            command.Ukprn = ukprn;
            await _mediator.Send(command);
            return NoContent();
        }
    }
}