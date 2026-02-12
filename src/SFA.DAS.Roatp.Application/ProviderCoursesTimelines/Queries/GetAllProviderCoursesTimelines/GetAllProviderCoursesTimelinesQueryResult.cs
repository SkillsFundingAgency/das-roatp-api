using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Roatp.Domain.Entities;

namespace SFA.DAS.Roatp.Application.ProviderCoursesTimelines.Queries.GetAllProviderCoursesTimelines;

public class GetAllProviderCoursesTimelinesQueryResult
{
    public IEnumerable<ProviderCoursesTimelineModel> Providers { get; set; } = [];

    public static implicit operator GetAllProviderCoursesTimelinesQueryResult(List<ProviderRegistrationDetail> providers)
    {
        GetAllProviderCoursesTimelinesQueryResult result = new()
        {
            Providers = providers.Select(p => (ProviderCoursesTimelineModel)p)
        };
        return result;
    }
}


