using System.Threading.Tasks;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Roatp.Application.AcademicYears.Queries.GetLatest;
using static SFA.DAS.Roatp.Api.Infrastructure.Constants;

namespace SFA.DAS.Roatp.Api.Controllers.ExternalReadControllers;

[ApiController]
[ApiVersion(ApiVersionNumber.One)]
[ApiVersion(ApiVersionNumber.Two)]
[Route("/api/[controller]/")]
public class AcademicYearsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AcademicYearsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [MapToApiVersion(ApiVersionNumber.One)]
    [MapToApiVersion(ApiVersionNumber.Two)]
    [Route("latest")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(GetAcademicYearsLatestQueryResult), 200)]
    public async Task<ActionResult> GetLatestAcademicYears()
    {
        var result = await _mediator.Send(new GetAcademicYearsLatestQuery());
        return new OkObjectResult(result);
    }
}
