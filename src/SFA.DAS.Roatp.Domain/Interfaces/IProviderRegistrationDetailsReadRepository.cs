using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Roatp.Domain.Entities;

namespace SFA.DAS.Roatp.Domain.Interfaces;

public interface IProviderRegistrationDetailsReadRepository
{
    Task<List<ProviderRegistrationDetail>> GetActiveProviderRegistrations(CancellationToken cancellationToken);
    Task<List<ProviderRegistrationDetail>> GetActiveAndMainProviderRegistrations(CancellationToken cancellationToken);
    Task<ProviderRegistrationDetail> GetProviderRegistrationDetail(int ukprn);

    Task<bool> IsMainActiveProvider(int ukprn, int larsCode);
}