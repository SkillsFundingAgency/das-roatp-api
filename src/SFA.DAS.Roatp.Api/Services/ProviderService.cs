using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Roatp.Api.Models;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Api.Services
{
    public class ProviderService : IProviderService
    {
        private readonly IProviderRepository _providerRepository;

        public ProviderService(IProviderRepository providerRepository)
        {
            _providerRepository = providerRepository;
        }

        public async Task<Provider> GetProvider(int ukprn)
        {
            var provider = await _providerRepository.GetProvider(ukprn);
            if (provider == null) return null;
            return provider;
        }

        public async Task<Provider> UpdateProvider(int ukprn, bool hasConfirmedDetails)
        {
            var provider = await _providerRepository.UpdateProvider(ukprn, hasConfirmedDetails);
            if (provider == null) return null;
            return provider;
        }
    }
}
