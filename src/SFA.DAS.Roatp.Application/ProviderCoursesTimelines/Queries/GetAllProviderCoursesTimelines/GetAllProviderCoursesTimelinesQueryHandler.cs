using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.ProviderCoursesTimelines.Queries.GetAllProviderCoursesTimelines;

public class GetAllProviderCoursesTimelinesQueryHandler(IProviderCoursesTimelineRepository _providerCoursesTimelineRepository) : IRequestHandler<GetAllProviderCoursesTimelinesQuery, GetAllProviderCoursesTimelinesQueryResult>
{
    public async Task<GetAllProviderCoursesTimelinesQueryResult> Handle(GetAllProviderCoursesTimelinesQuery request, CancellationToken cancellationToken)
    {
        List<ProviderRegistrationDetail> providers = await _providerCoursesTimelineRepository.GetAllProviderCoursesTimelines(cancellationToken);

        GetAllProviderCoursesTimelinesQueryResult result = providers;

        return result;
    }
}

