using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Jobs.Services;

namespace SFA.DAS.Roatp.Jobs.Functions
{
    public class LoadAllProviderAddressesFunction
    {
        private readonly ILoadUkrlpAddressesService _loadUkrlpAddressesService;
        private readonly ILogger<LoadAllProviderAddressesFunction> _logger;

        public LoadAllProviderAddressesFunction(ILoadUkrlpAddressesService loadUkrlpAddressesService, ILogger<LoadAllProviderAddressesFunction> logger)
        {
            _loadUkrlpAddressesService = loadUkrlpAddressesService;
            _logger = logger;
        }

        [Function(nameof(LoadAllProviderAddressesFunction))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "POST", Route = "LoadAllProviderAddresses")] HttpRequest req)
        {
            var result = await _loadUkrlpAddressesService.LoadAllProvidersAddresses();
            if (result)
                _logger.LogInformation("Ukrlp Addresses updated");
            else
                _logger.LogWarning("Ukrlp addresses not updated");

            return new OkObjectResult(result);
        }
    }
}
