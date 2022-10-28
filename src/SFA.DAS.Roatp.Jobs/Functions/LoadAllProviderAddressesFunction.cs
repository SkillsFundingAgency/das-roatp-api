using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Jobs.Services;

namespace SFA.DAS.Roatp.Jobs.Functions
{
    public class LoadAllProviderAddressesFunction
    {
        private readonly ILoadUkrlpAddressesService _loadUkrlpAddressesService;

        public LoadAllProviderAddressesFunction(ILoadUkrlpAddressesService loadUkrlpAddressesService)
        {
            _loadUkrlpAddressesService = loadUkrlpAddressesService;
        }

        [FunctionName(nameof(LoadAllProviderAddressesFunction))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "POST", Route = "LoadAllProviderAddresses")] HttpRequest req, ILogger log)
        {
            var result = await _loadUkrlpAddressesService.LoadAllProvidersAddresses();
            if (result)
                log.LogInformation("Ukrlp Addresses updated");
            else
                log.LogWarning("Ukrlp addresses not updated");

            return new OkObjectResult(result);
        }
    }
}
