using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Application.OverallNationalAchievementRates.Queries;
using SFA.DAS.Roatp.Application.OverallNationalAchievementRates.Queries.GetOverallAchievementRates;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Api.Controllers.ExternalReadControllers
{
    [ApiVersion("1.0")]
    [ApiController]
    [Route("/api/[controller]/")]
    public class AchievementRatesController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<AchievementRatesController> _logger;

        public AchievementRatesController (IMediator mediator, ILogger<AchievementRatesController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet]
        [Route("Overall")]
        public async Task<IActionResult> GetOverallAchievementRates([FromQuery] string sector)
        {
            _logger.LogInformation("Request received to get overall achievement rates by:{sector}", sector);

            var queryResult = await _mediator.Send(new GetOverallAchievementRatesQuery { SectorSubjectArea = sector });
            
            _logger.LogInformation("Found {overallAchievementRatesCount} overall achievement rates", queryResult.OverallAchievementRates.Count);

            return Ok(queryResult);
        }
    }
}