using System.Collections.Generic;
using SFA.DAS.Roatp.Domain.Models;

namespace SFA.DAS.Roatp.Application.Courses.Queries.GetProvidersFromLarsCode.V2
{
    public class GetProvidersForLarsCodeQueryResultV2
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public int TotalCount { get; set; }
        public string LarsCode { get; set; }
        public string StandardName { get; set; }
        public string QarPeriod { get; set; }
        public string ReviewPeriod { get; set; }

        public List<ProviderDataV2> Providers { get; set; }
    }
}