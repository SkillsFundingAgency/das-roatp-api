using System.Threading.Tasks;
using SFA.DAS.Roatp.Jobs.Infrastructure.ApiClients.StandardsApi.Models;

namespace SFA.DAS.Roatp.Jobs.Infrastructure.ApiClients.StandardsApi
{
    public interface IGetActiveStandardsApiClient
    {
        Task<StandardList> GetActiveStandards();
    }
}
