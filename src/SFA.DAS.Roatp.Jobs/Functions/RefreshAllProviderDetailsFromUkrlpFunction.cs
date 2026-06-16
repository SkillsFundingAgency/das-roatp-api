using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using SFA.DAS.Roatp.Jobs.Services;

namespace SFA.DAS.Roatp.Jobs.Functions;

public class RefreshAllProviderDetailsFromUkrlpFunction(IRefreshProviderDetailsFromUkrlpService _refreshProviderDetailsFromUkrlpService)
{
    [Function(nameof(RefreshAllProviderDetailsFromUkrlpFunction))]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "GET", Route = "refresh-providers-data")] HttpRequest req)
    {
        await _refreshProviderDetailsFromUkrlpService.RefreshProviderDetailsFromUkrlp(false);
        return new OkObjectResult("Function to update provider details completed successfully");
    }
}
