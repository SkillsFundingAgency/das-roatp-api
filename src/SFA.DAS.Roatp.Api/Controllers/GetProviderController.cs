using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Domain.Entities;

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
        public Task<Provider> Get()
        {
            return Task.FromResult(new Provider());
        }
    }
}
