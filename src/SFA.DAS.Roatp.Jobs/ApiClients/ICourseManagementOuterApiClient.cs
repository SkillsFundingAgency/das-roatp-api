using System.Net;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Jobs.ApiClients
{
    public interface ICourseManagementOuterApiClient
    {
        Task<(bool, T)> Get<T>(string uri);
    }
}