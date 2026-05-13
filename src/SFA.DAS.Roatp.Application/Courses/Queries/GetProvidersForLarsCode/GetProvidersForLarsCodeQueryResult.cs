using System.Collections.Generic;
using SFA.DAS.Roatp.Domain.Models;

namespace SFA.DAS.Roatp.Application.Courses.Queries.GetProvidersForLarsCode;

public class GetProvidersForLarsCodeQueryResult : ProviderSearchResultSummary
{
    public List<ProviderData> Providers { get; set; } = [];
}