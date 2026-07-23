using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Roatp.Domain.Entities;

namespace SFA.DAS.Roatp.Application.ProviderCoursesTimelines.Queries.GetAllProviderCoursesTimelines;

public class GetAllProviderCoursesTimelinesQueryResult
{
    public IEnumerable<ProviderCoursesTimelineModel> Providers { get; set; } = [];

    public static implicit operator GetAllProviderCoursesTimelinesQueryResult(List<ProviderTimelineExport> providers)
    {
        GetAllProviderCoursesTimelinesQueryResult result = new()
        {
            Providers = providers.GroupBy(x => x.Ukprn)
                .Select(p => (ProviderCoursesTimelineModel)p.ToList())
        };
        return result;
    }
}


