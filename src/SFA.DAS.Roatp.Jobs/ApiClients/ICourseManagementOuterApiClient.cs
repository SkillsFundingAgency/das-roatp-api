using System.Net;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Jobs.ApiClients
{
    public interface ICourseManagementOuterApiClient
    {
        Task<(bool, T)> Get<T>(string uri);
        // Task<(bool, T,U)> Post<T, U>(string uri, T model);
        Task<(bool,U)> Post<T, U>(string uri, T model);
    }
}