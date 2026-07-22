using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Roatp.Application.ProviderCoursesTimelines.Queries;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.UnitTests.ProviderCoursesTimelines.Queries.GetProviderCoursesTimelines;

public class GetProviderCoursesTimelinesQueryHandler(IProviderCoursesTimelineRepository _providerCoursesTimelineRepository) : IRequestHandler<GetProviderCoursesTimelinesQuery, ProviderCoursesTimelineModel>
{
    public async Task<ProviderCoursesTimelineModel> Handle(GetProviderCoursesTimelinesQuery request, CancellationToken cancellationToken)
    {
        var providerTimelines = await _providerCoursesTimelineRepository.GetProviderCoursesTimeline(request.Ukprn, cancellationToken);

        return providerTimelines;
    }
}
