using System.Collections.Generic;
using SFA.DAS.Roatp.Jobs.Infrastructure.ApiClients.StandardsApi.Models;

namespace SFA.DAS.Roatp.Jobs.Infrastructure.ApiClients.RoatpV2Api.Models
{
    public class StandardsRequest
    {
        public List<Standard> Standards { get; set; }
    }
}
