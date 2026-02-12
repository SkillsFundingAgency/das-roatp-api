using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Roatp.Domain.Entities;

namespace SFA.DAS.Roatp.Domain.Interfaces;

public interface IProviderCoursesTimelineRepository
{
    Task<List<ProviderRegistrationDetail>> GetAllProviderCoursesTimelines(CancellationToken cancellationToken);
    Task<ProviderRegistrationDetail> GetProviderCoursesTimelines(int ukprn, CancellationToken cancellationToken);
}
