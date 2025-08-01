using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Roatp.Api.Infrastructure;
using SFA.DAS.Roatp.Application.ProviderContact.Queries.GetProviderContact;

namespace SFA.DAS.Roatp.Api.Controllers;

[ApiController]
[Route("/providers/{ukprn}/contact")]
public class ProviderContactController(IMediator _mediator) : ActionResponseControllerBase

{
    /// <summary>
    /// Gets latest contact details for the given provider
    /// </summary>
    /// <param name="ukprn"></param>
    /// <returns></returns>
    [HttpGet]
    [Produces("application/json")]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(GetLatestProviderContactQueryResult), 200)]
    public async Task<IActionResult> GetLatestProviderContact(int ukprn)
    {
        var response = await _mediator.Send(new GetLatestProviderContactQuery(ukprn));

        return GetResponse(response);
    }
}