using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace SFA.DAS.Roatp.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GetProviderController : ControllerBase
    {
        private readonly ILogger<GetProviderController> _logger;

        public GetProviderController(ILogger<GetProviderController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Get() => Ok();
    }
}
