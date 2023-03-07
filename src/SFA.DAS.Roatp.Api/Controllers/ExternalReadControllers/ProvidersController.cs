using System.Collections.Generic;
using System.Linq;
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
        [ProducesResponseType(typeof(GetProvidersQueryResult), 200)]
        public async Task<IActionResult> GetProviders()
        {
            var providerResult = await _mediator.Send(new GetProvidersQuery());
            _logger.LogInformation("Providers summary data found");
            return new OkObjectResult(providerResult);
        }

        [HttpGet]
        [Route("{ukprn}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ProviderSummary), 200)]
        public async Task<IActionResult> GetProvider(int ukprn)
        {
            var response = await _mediator.Send(new GetProviderSummaryQuery(ukprn));

            if (response == null) return NotFound();

            if (response.IsValidResponse)
            {
                _logger.LogInformation("Provider summary data found for {ukprn}:", ukprn);
                return new OkObjectResult(response.Result);
            }

            var formattedErrors =  response.Errors.Select(err => new ValidationError
            {
                PropertyName = err.PropertyName,
                ErrorMessage = err.ErrorMessage
            }).ToList();

            return new BadRequestObjectResult(formattedErrors);
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
        [ProducesResponseType(typeof(List<ProviderCourseModel>), 200)]
        public async Task<IActionResult> GetAllProviderCourses(int ukprn)
        {
            var response = await _mediator.Send(new GetAllProviderCoursesQuery(ukprn));
            if (response.IsValidResponse)
                _logger.LogInformation("{count} Provider courses found for {ukprn}:", response.Result.Count, ukprn);
            return GetResponse(response);
        }

        [HttpGet]
        [Route("{ukprn}/courses/{larsCode}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProviderCourseModel), 200)]
        public async Task<IActionResult> GetProviderCourse(int ukprn, int larsCode)
        {
            var response = await _mediator.Send(new GetProviderCourseQuery(ukprn, larsCode));
            if (response.IsValidResponse)
                _logger.LogInformation("Course data found for {ukprn} and {larsCode}", ukprn, larsCode);
            return GetResponse(response);
        }
    }
}
