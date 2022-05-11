using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Jobs.Infrastructure.ApiClients.RoatpV2Api.Models;

namespace SFA.DAS.Roatp.Jobs.Infrastructure.ApiClients.RoatpV2Api
{
    [ExcludeFromCodeCoverage]
    public class ReloadStandardsApiClient : ApiClientBase<ReloadStandardsApiClient>, IReloadStandardsApiClient
    {
        public ReloadStandardsApiClient(HttpClient client, ILogger<ReloadStandardsApiClient> logger)
            : base(client, logger)
        {
        }

        public async Task<HttpStatusCode> ReloadStandardsDetails(StandardsRequest standardsRequest)
        {
            var url = "ReloadStandardsData";
            return await Post(url, standardsRequest);
        }
    }
}