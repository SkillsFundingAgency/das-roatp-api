using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Roatp.Application.ProviderCoursesTimelines.Queries;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.UnitTests.ProviderCoursesTimelines.Queries.GetProviderCoursesTimelines;

public class GetProviderCoursesTimelinesQueryHandler(IProviderCoursesTimelineRepository _providerCoursesTimelineRepository) : IRequestHandler<GetProviderCoursesTimelinesQuery, ProviderCoursesTimelineModel>
{
    public async Task<ProviderCoursesTimelineModel> Handle(GetProviderCoursesTimelinesQuery request, CancellationToken cancellationToken)
    {
        ProviderRegistrationDetail provider = await _providerCoursesTimelineRepository.GetProviderCoursesTimelines(request.Ukprn, cancellationToken);
        return (ProviderCoursesTimelineModel)provider;
    }
}
