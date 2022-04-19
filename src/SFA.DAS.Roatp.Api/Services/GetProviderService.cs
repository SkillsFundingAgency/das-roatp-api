using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Api.Services
{
    public class GetProviderService : IGetProviderService
    {
        private readonly IProviderReadRepository _providerReadRepository;
        private readonly ILogger<GetProviderService> _logger;

        public GetProviderService(IProviderReadRepository providerReadRepository, ILogger<GetProviderService> logger)
        {
            _providerReadRepository = providerReadRepository;
        }

        public async Task<Provider> GetProvider(int ukprn)
        {
            var provider = await _providerReadRepository.GetProvider(ukprn);
            if (provider == null)
            {
                _logger.LogWarning("Provider with UKPRN {ukprn} was not found.", ukprn);
                return null;
            }
            return provider;
        }
    }
}
