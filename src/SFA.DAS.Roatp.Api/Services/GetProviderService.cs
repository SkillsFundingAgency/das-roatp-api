using System.Threading.Tasks;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Api.Services
{
    public class GetProviderService : IGetProviderService
    {
        private readonly IProviderReadRepository _providerReadRepository;

        public GetProviderService(IProviderReadRepository providerReadRepository)
        {
            _providerReadRepository = providerReadRepository;
        }

        public async Task<Provider> GetProvider(int ukprn)
        {
            var provider = await _providerReadRepository.GetProvider(ukprn);
            if (provider == null) return null;
            return provider;
        }
    }
}
