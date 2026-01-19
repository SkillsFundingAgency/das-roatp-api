using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Api.Infrastructure;
using SFA.DAS.Roatp.Application.Mediatr.Responses;
using SFA.DAS.Roatp.Application.ProviderCourse.Queries.ExternalRead.GetProviderCourse;
using SFA.DAS.Roatp.Application.ProviderCourse.Queries.GetAllProviderCourses;
using SFA.DAS.Roatp.Application.ProviderCourse.Queries.GetProviderCourse;
using SFA.DAS.Roatp.Application.Providers.Queries.GetProviders;
using SFA.DAS.Roatp.Application.Providers.Queries.GetProviderSummary;
using SFA.DAS.Roatp.Application.Providers.Queries.GetRegisteredProvider;
using SFA.DAS.Roatp.Domain.Models;
using static SFA.DAS.Roatp.Api.Infrastructure.Constants;

namespace SFA.DAS.Roatp.Api.Controllers.ExternalReadControllers
{
    [ApiController]
    [ApiVersion(ApiVersionNumber.One)]
    [ApiVersion(ApiVersionNumber.Two)]
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
        [MapToApiVersion(ApiVersionNumber.Two)]
        [Route("GetV2")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public IActionResult GetV2() => Ok("v2");

        [HttpGet]
        [MapToApiVersion(ApiVersionNumber.One)]
        [MapToApiVersion(ApiVersionNumber.Two)]
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
        [MapToApiVersion(ApiVersionNumber.One)]
        [MapToApiVersion(ApiVersionNumber.Two)]
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
        [MapToApiVersion(ApiVersionNumber.One)]
        [MapToApiVersion(ApiVersionNumber.Two)]
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
        [MapToApiVersion(ApiVersionNumber.One)]
        [MapToApiVersion(ApiVersionNumber.Two)]
        [Route("{ukprn}/courses")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(List<ProviderCourseModelExternal>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllProviderCourses(int ukprn)
        {
            var providersResponse = await _mediator.Send(new GetAllProviderCoursesQuery(ukprn, true, null));
            if (!providersResponse.IsValidResponse)
            {
                var errorsResponse =
                    new ValidatedResponse<List<ProviderCourseModelExternal>>(providersResponse.Errors.ToList());
                return GetResponse(errorsResponse);
            }

            _logger.LogInformation("{Count} Provider courses found for {Ukprn}:", providersResponse.Result.Count,
                    ukprn);

            var providerCourseModels = providersResponse.Result
                .Where(x => x.CourseType == CourseType.Apprenticeship)
                .Select(provider => (ProviderCourseModelExternal)provider).ToList();

            var response = new ValidatedResponse<List<ProviderCourseModelExternal>>(providerCourseModels);

            return GetResponse(response);
        }

        [HttpGet]
        [MapToApiVersion(ApiVersionNumber.One)]
        [MapToApiVersion(ApiVersionNumber.Two)]
        [Route("{ukprn}/courses/{larsCode}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProviderCourseModelExternal), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetProviderCourse(int ukprn, int larsCode)
        {
            var providerResponse = await _mediator.Send(new GetProviderCourseQuery(ukprn, larsCode.ToString()));
            if (!providerResponse.IsValidResponse)
            {
                var errorsResponse =
                    new ValidatedResponse<ProviderCourseModelExternal>(providerResponse.Errors.ToList());
                return GetResponse(errorsResponse);
            }

            _logger.LogInformation("Course data found for {Ukprn} and {LarsCode}", ukprn, larsCode);

            var response = new ValidatedResponse<ProviderCourseModelExternal>(providerResponse.Result);

            return GetResponse(response);
        }
    }
}
