using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Jobs.ApiClients;

namespace SFA.DAS.Roatp.Jobs.Functions;

public class DeleteExpiredShortlistsFunction(ILogger<DeleteExpiredShortlistsFunction> _logger, ICourseManagementOuterApiClient _courseManagementOuterApiClient)
{
    public const string DeleteExpiredShortlistsUri = "shortlists/expired";

    [Function(nameof(DeleteExpiredShortlistsFunction))]
    public async Task Run([TimerTrigger("%DeleteExpiredShortlistsSchedule%", RunOnStartup = true)] TimerInfo myTimer, CancellationToken cancellationToken)
    {
        _logger.LogInformation("DeleteExpiredShortlistsFunction started");

        await _courseManagementOuterApiClient.Delete(DeleteExpiredShortlistsUri, cancellationToken);

        _logger.LogInformation("DeleteExpiredShortlistsFunction completed");
    }
}
