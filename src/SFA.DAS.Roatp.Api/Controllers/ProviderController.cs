using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Api.Models;
using SFA.DAS.Roatp.Api.Services;
using SFA.DAS.Roatp.Domain.Entities;

namespace SFA.DAS.Roatp.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProviderController : ControllerBase
    {
        private readonly ILogger<ProviderController> _logger;
        private readonly IProviderService _providerService;

        public ProviderController(
            ILogger<ProviderController> logger,
            IProviderService providerService)
        {
            _logger = logger;
            _providerService = providerService;
        }


        /// <summary>
        /// Gets the available provider for the given ukprn
        /// </summary>
        /// <param name="ukprn"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("/providers/{ukprn}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Provider), 200)]
        public async Task<ActionResult<Provider>> GetProvider(int ukprn)
        {
            if (ukprn <= 0 ) return new BadRequestObjectResult("Invalid ukprn.");

            var result = await _providerService.GetProvider(ukprn);

            if (result == null) return new NotFoundObjectResult($"No data found for {ukprn}");

            return new OkObjectResult(result);
        }

        /// <summary>
        /// Gets all the available delivery models for the given provider and standard
        /// </summary>
        /// <param name="ukprn"></param>
        /// <param name="larsCode"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("/providers/{ukprn}/{hasConfirmedDetails}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProviderCourseModel), 200)]
        public async Task<ActionResult<Provider>> UpdateProvider(int ukprn, bool hasConfirmedDetails)
        {
            if (ukprn <= 0) return new BadRequestObjectResult("Invalid ukprn.");

            var result = await _providerService.UpdateProvider(ukprn, hasConfirmedDetails);

            if (result == null) return new NotFoundObjectResult($"No data found for {ukprn}");

            return new OkObjectResult(result);
        }
    }
}
