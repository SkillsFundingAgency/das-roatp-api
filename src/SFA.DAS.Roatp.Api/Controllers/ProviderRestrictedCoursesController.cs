using System.Threading.Tasks;
using Asp.Versioning;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Api.Infrastructure;
using SFA.DAS.Roatp.Api.Models;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Application.ProviderRestrictedCourses.Queries.GetProviderRestrictedApprenticeships;
using static SFA.DAS.Roatp.Api.Infrastructure.Constants;

namespace SFA.DAS.Roatp.Api.Controllers;

[ApiController]
[ApiVersion(ApiVersionNumber.One)]
[Route("providers/{ukprn}")]
public class ProviderRestrictedCoursesController(IMediator _mediator, ILogger<ProviderRestrictedCoursesController> _logger, IValidator<IUkprn> _ukprnValidator) : ActionResponseControllerBase
{
    [HttpGet("restricted-apprenticeships")]
    [ProducesResponseType(typeof(GetProviderRestrictedApprenticeshipsQueryResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetRestrictedApprenticeships([FromRoute] int ukprn)
    {
        _logger.LogInformation("Request received to get restricted apprenticeships for ukprn: {Ukprn}", ukprn);

        var model = new UkprnValidatorModel { Ukprn = ukprn };

        var validationResult = await _ukprnValidator.ValidateAsync(model);

        if (!validationResult.IsValid)
        {
            return NotFound(FormatErrors(validationResult.Errors));
        }

        GetProviderRestrictedApprenticeshipsQuery query = new() { Ukprn = ukprn };
        var result = await _mediator.Send(query);
        return GetResponse(result);
    }
}
