using SFA.DAS.Roatp.Domain.Models;
using System.Collections.Generic;

namespace SFA.DAS.Roatp.Application.Courses.Queries.GetProvidersFromLarsCode
{
    public class GetProvidersForLarsCodeQueryResult
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public int TotalCount { get; set; }
        public int LarsCode { get; set; }
        public string StandardName { get; set; }
        public string QarPeriod { get; set; }
        public string ReviewPeriod { get; set; }

        public List<ProviderData> Providers { get; set; }
    }
}