using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Roatp.Api.Infrastructure;
using SFA.DAS.Roatp.Application.Mediatr.Responses;
using SFA.DAS.Roatp.Application.Shortlists.Commands.CreateShortlist;
using SFA.DAS.Roatp.Application.Shortlists.Queries.GetShortlistCountForUser;

namespace SFA.DAS.Roatp.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ShortlistsController(IMediator _mediator) : ActionResponseControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(CreateShortlistCommandResult), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(CreateShortlistCommandResult), StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(IEnumerable<ValidationFailure>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateShortlist([FromBody] CreateShortlistCommand command, CancellationToken cancellationToken)
    {
        ValidatedResponse<CreateShortlistCommandResult> validatedResponse = await _mediator.Send(command, cancellationToken);
        var uri = validatedResponse.IsValidResponse ? $"api/shortlists/{validatedResponse.Result.ShortlistId}" : string.Empty;

        if (validatedResponse.IsValidResponse && !validatedResponse.Result.IsCreated) return NoContent();
        return GetPostResponse(validatedResponse, uri);
    }

    [HttpGet]
    [Route("{id}")]
    public IActionResult GetShortlist([FromRoute] string id, CancellationToken cancellationToken)
    {
        return Ok("not implemented");
    }

    [HttpGet]
    [Route("users/{userId}/count")]
    [ProducesResponseType(typeof(IEnumerable<ValidationFailure>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(GetShortlistsCountForUserQueryResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetShortlistsCountForUser([FromRoute] Guid userId, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetShortlistsCountForUserQuery(userId), cancellationToken);
        return GetResponse(result);
    }
}
