using SFA.DAS.Roatp.Domain.Entities;

namespace SFA.DAS.Roatp.Jobs.Services;
public interface IImportAnnualFeedbackSummariesService
{
    string GetTimePeriod();
    Task<bool> CheckIfDataExists(string timePeriod);
    Task<(IEnumerable<ProviderApprenticeStars>, IEnumerable<ProviderEmployerStars>)> GetFeedbackSummaries(string timePeriod);
}
