using System.Collections.Generic;
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
using SFA.DAS.Roatp.Application.ProviderCourseTypes.Commands.CreateProviderCourseType;
using SFA.DAS.Roatp.Application.ProviderCourseTypes.Commands.RestrictProvider;
using SFA.DAS.Roatp.Application.ProviderCourseTypes.Queries.GetProviderCourseTypes;
using SFA.DAS.Roatp.Domain.Models;
using static SFA.DAS.Roatp.Api.Infrastructure.Constants;

namespace SFA.DAS.Roatp.Api.Controllers;

[ApiController]
[ApiVersion(ApiVersionNumber.One)]
[Tags(EndpointTags.ProviderCourses)]
[Route("/providers/{ukprn}/course-types", Name = RouteNames.GetProviderCourseTypes)]
public class ProviderCourseTypesController(IMediator _mediator, ILogger<ProviderCourseTypesController> _logger, IValidator<IUkprn> _ukprnValidator) : ActionResponseControllerBase
{
    [HttpGet]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(List<ProviderCourseTypeModel>), 200)]
    public async Task<IActionResult> GetProviderCourseTypes(int ukprn)
    {
        _logger.LogInformation("Request received to get all provider Course Types for ukprn: {Ukprn}", ukprn);

        var response = await _mediator.Send(new GetProviderCourseTypesQuery(ukprn));

        if (response.IsValidResponse)
            _logger.LogInformation("Found {Count} courseTypes for ukprn: {Ukprn}", response.Result.Count, ukprn);

        return GetResponse(response);
    }

    [HttpPost("{courseType}/restrict")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> RestrictProvider([FromRoute] int ukprn, [FromRoute] CourseType courseType, [FromBody] RestrictProviderModel request)
    {
        _logger.LogInformation("Request to restrict provider for Ukprn {Ukprn} and CourseType {CourseType}", ukprn, courseType);

        RestrictProviderCommand command = new()
        {
            Ukprn = ukprn,
            CourseType = courseType,
            UserId = request.UserId,
            UserDisplayName = request.UserDisplayName
        };

        var response = await _mediator.Send(command);

        return GetNoContentResponse(response);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> AddCourseTypes([FromRoute] int ukprn, [FromBody] AddCourseTypesModel request)
    {
        _logger.LogInformation("Request to add course types for Ukprn {Ukprn}", ukprn);

        var model = new UkprnValidatorModel
        {
            Ukprn = ukprn,
        };

        var result = await _ukprnValidator.ValidateAsync(model);

        if (!result.IsValid)
        {
            return NotFound(FormatErrors(result.Errors));
        }

        CreateProviderCourseTypeCommand command = new()
        {
            Ukprn = ukprn,
            CourseTypes = request.CourseTypes,
            UserId = request.UserId,
            UserDisplayName = request.UserDisplayName
        };

        var response = await _mediator.Send(command);

        return GetNoContentResponse(response);
    }
}
