using System.Collections.Generic;
using System.Threading.Tasks;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Application.OverallNationalAchievementRates.Queries.GetOverallAchievementRates;
using SFA.DAS.Roatp.Domain.Entities;

namespace SFA.DAS.Roatp.Api.Controllers.ExternalReadControllers;

[ApiController]
[ApiVersion("1.0")]
[ApiVersion("2.0")]
[Route("/api/[controller]/")]
public class AchievementRatesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<AchievementRatesController> _logger;

    public AchievementRatesController(IMediator mediator, ILogger<AchievementRatesController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet]
    [Route("Overall")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(List<NationalAchievementRateOverall>), 200)]
    public async Task<IActionResult> GetOverallAchievementRates([FromQuery] int sectorSubjectAreaTier1Code)
    {
        _logger.LogInformation("Request received to get overall achievement rates by:{Sector}", sectorSubjectAreaTier1Code);

        var queryResult = await _mediator.Send(new GetOverallAchievementRatesQuery { SectorSubjectAreaTier1Code = sectorSubjectAreaTier1Code });

        _logger.LogInformation("Found {OverallAchievementRatesCount} overall achievement rates", queryResult.OverallAchievementRates.Count);

        return Ok(queryResult);
    }
}