using SFA.DAS.Roatp.Jobs.ApiClients;
using SFA.DAS.Roatp.Jobs.ApiModels;
using SFA.DAS.Roatp.Jobs.Configuration;

namespace SFA.DAS.Roatp.Jobs.Services;

public class ProviderEmailProcessingService(ICourseManagementOuterApiClient _courseManagementOuterApiClient) : IProviderEmailProcessingService
{
    public async Task SendEmailsInBatches(IEnumerable<ProviderEmailModel> models)
    {
        var batches = models.Chunk(ForecastEmailConfiguration.BatchSize);

        foreach (var batch in batches) await ProcessBatch(batch);
    }

    private async Task ProcessBatch(ProviderEmailModel[] batch)
    {
        List<Task> tasks = [];
        foreach (ProviderEmailModel model in batch)
        {
            var ukprn = model.Tokens[ProviderEmailTokens.Ukprn];
            tasks.Add(_courseManagementOuterApiClient.Post($"providers/{ukprn}/emails", model));
        }

        tasks.Add(Task.Delay(ForecastEmailConfiguration.EmailThrottlingInSeconds));
        await Task.WhenAll(tasks);
    }
}
