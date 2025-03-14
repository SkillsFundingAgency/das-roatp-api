using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Roatp.Application.AcademicYears.Queries.GetLatest;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Api.Controllers.ExternalReadControllers;

[ApiController]
[Route("/api/[controller]/")]
public class AcademicYearsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AcademicYearsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
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
