using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Domain.Interfaces
{
    public interface IProvidersReadRepository
    {
        Task<Provider> GetByUkprn(int ukprn);
        Task<List<Provider>> GetAllProviders();

        Task<List<ProviderSearchModel>> GetProvidersByLarsCode(int larsCode, ProviderOrderBy sortOrder, GetProvidersFromLarsCodeOptionalParameters parameters, CancellationToken cancellationToken);

    }
}