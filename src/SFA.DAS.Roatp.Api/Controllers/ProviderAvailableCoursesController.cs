using System.Threading;
using System.Threading.Tasks;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Roatp.Api.Infrastructure;
using SFA.DAS.Roatp.Application.Mediatr.Responses;
using SFA.DAS.Roatp.Application.ProviderCourse.Queries.GetProviderAvailableCourses;
using SFA.DAS.Roatp.Domain.Models;
using static SFA.DAS.Roatp.Api.Infrastructure.Constants;

namespace SFA.DAS.Roatp.Api.Controllers;

[ApiController]
[ApiVersion(ApiVersionNumber.One)]
[Tags(EndpointTags.ProviderCourses)]
[Route("providers/{ukprn}/course-types/{courseType}")]
public class ProviderAvailableCoursesController(IMediator _mediator) : ActionResponseControllerBase
{
    [HttpGet]
    [Route("available-courses")]
    [ProducesResponseType(typeof(ValidatedResponse<GetProviderAvailableCoursesQueryResult>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAvailableCourses([FromRoute] int ukprn, [FromRoute] CourseType courseType, CancellationToken cancellationToken)
    {
        GetProviderAvailableCoursesQuery query = new(ukprn, courseType);
        ValidatedResponse<GetProviderAvailableCoursesQueryResult> result = await _mediator.Send(query, cancellationToken);
        return GetResponse(result);
    }
}
