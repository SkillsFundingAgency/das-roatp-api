using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Application.OverallNationalAchievementRates.Queries.GetOverallAchievementRates;
using SFA.DAS.Roatp.Application.ProviderCourse.Queries.GetAllProviderCourses;
using SFA.DAS.Roatp.Application.Providers.Queries.GetProviders;
using SFA.DAS.Roatp.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Api.Controllers.ExternalReadControllers
{
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
        [Produces("application/json")]
        [ProducesResponseType(typeof(List<NationalAchievementRateOverall>), 200)]
        public async Task<IActionResult> GetOverallAchievementRates([FromQuery] string sector)
        {
            _logger.LogInformation("Request received to get overall achievement rates by:{sector}", sector);

            var queryResult = await _mediator.Send(new GetOverallAchievementRatesQuery { SectorSubjectArea = sector });
            
            _logger.LogInformation("Found {overallAchievementRatesCount} overall achievement rates", queryResult.OverallAchievementRates.Count);

            return Ok(queryResult);
        }
    }
}