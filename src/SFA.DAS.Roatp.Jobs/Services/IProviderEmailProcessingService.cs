using SFA.DAS.Roatp.Jobs.ApiModels;

namespace SFA.DAS.Roatp.Jobs.Services;

public interface IProviderEmailProcessingService
{
    Task SendEmailsInBatches(IEnumerable<ProviderEmailModel> models);
}
