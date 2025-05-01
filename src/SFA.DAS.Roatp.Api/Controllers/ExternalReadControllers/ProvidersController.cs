using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Api.Infrastructure;
using SFA.DAS.Roatp.Application.ProviderCourse.Queries.GetAllProviderCourses;
using SFA.DAS.Roatp.Application.ProviderCourse.Queries.GetProviderCourse;
using SFA.DAS.Roatp.Application.Providers.Queries.GetProviders;
using SFA.DAS.Roatp.Application.Providers.Queries.GetProviderSummary;
using SFA.DAS.Roatp.Application.Providers.Queries.GetRegisteredProvider;

namespace SFA.DAS.Roatp.Api.Controllers.ExternalReadControllers
{
    [ApiController]
    [Route("/api/[controller]/")]
    public class ProvidersController : ActionResponseControllerBase
    {
        private readonly ILogger<ProvidersController> _logger;
        private readonly IMediator _mediator;

        public ProvidersController(ILogger<ProvidersController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [HttpGet]
        [Route("")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(GetProvidersQueryResult), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetProviders([FromQuery] bool? Live, CancellationToken cancellationToken)
        {
            var providerResult = await _mediator.Send(
                new GetProvidersQuery() { Live = Live ?? false },
                cancellationToken
            );

            _logger.LogInformation("Providers summary data found");
            return new OkObjectResult(providerResult);
        }

        [HttpGet]
        [Route("{ukprn:int}/summary")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(GetProviderSummaryQueryResult), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetProviderSummary([FromRoute] int ukprn)
        {
            return GetResponse(
                await _mediator.Send(new GetProviderSummaryQuery(ukprn))
            );
        }

        [HttpGet]
        [Route("{ukprn:int}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(GetProviderSummaryQueryResult), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetRegisteredProvider([FromRoute] int ukprn, CancellationToken cancellationToken)
        {
            return GetResponse(await _mediator.Send(new GetRegisteredProviderQuery(ukprn), cancellationToken));
        }

        /// <summary>
        /// Gets all the courses for the given provider
        /// </summary>
        /// <param name="ukprn"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{ukprn}/courses")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(List<ProviderCourseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllProviderCourses(int ukprn)
        {
            var response = await _mediator.Send(new GetAllProviderCoursesQuery(ukprn));
            if (response.IsValidResponse)
                _logger.LogInformation("{Count} Provider courses found for {Ukprn}:", response.Result.Count, ukprn);
            return GetResponse(response);
        }

        [HttpGet]
        [Route("{ukprn}/courses/{larsCode}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProviderCourseModel), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetProviderCourse(int ukprn, int larsCode)
        {
            var response = await _mediator.Send(new GetProviderCourseQuery(ukprn, larsCode));
            if (response.IsValidResponse)
                _logger.LogInformation("Course data found for {Ukprn} and {LarsCode}", ukprn, larsCode);
            return GetResponse(response);
        }
    }
}
