using System.Threading.Tasks;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Api.Infrastructure;
using SFA.DAS.Roatp.Application.Standards.Queries.GetAllStandards;
using SFA.DAS.Roatp.Application.Standards.Queries.GetStandardForLarsCode;
using SFA.DAS.Roatp.Domain.Models;
using static SFA.DAS.Roatp.Api.Infrastructure.Constants;

namespace SFA.DAS.Roatp.Api.Controllers
{
    [ApiController]
    [ApiVersion(ApiVersionNumber.One)]

    [Route("/standards")]
    public class StandardsController : ActionResponseControllerBase
    {
        private readonly ILogger<StandardsController> _logger;
        private readonly IMediator _mediator;

        public StandardsController(ILogger<StandardsController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [HttpGet]
        [Produces("application/json")]
        public async Task<ActionResult<GetAllStandardsQueryResult>> GetAllStandards([FromQuery] CourseType? courseType = null)
        {
            _logger.LogInformation("Inner API: Request received to get all standards");
            var result = await _mediator.Send(new GetAllStandardsQuery(courseType));
            return Ok(result);
        }

        [HttpGet]
        [Route("{larsCode}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(GetStandardForLarsCodeQueryResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetStandardForLarsCode([FromRoute] string larsCode)
        {
            _logger.LogInformation("Inner API: Request received to get standard for larsCode {LarsCode}", larsCode);
            var result = await _mediator.Send(new GetStandardForLarsCodeQuery(larsCode));
            return GetResponse(result);
        }
    }
}
