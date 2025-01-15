using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Jobs.Services;

namespace SFA.DAS.Roatp.Jobs.Functions;

public class LoadAllProviderAddressesFunction(ILoadUkrlpAddressesService _loadUkrlpAddressesService, ILogger<LoadAllProviderAddressesFunction> _logger)
{
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
