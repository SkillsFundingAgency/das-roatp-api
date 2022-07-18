using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Data.Repositories
{
    [ExcludeFromCodeCoverage]
    public class LoadProviderRepository : ILoadProviderRepository
    {
        private readonly RoatpDataContext _roatpDataContext;
        private readonly ILogger<LoadProviderRepository> _logger;

        public LoadProviderRepository(RoatpDataContext roatpDataContext, ILogger<LoadProviderRepository> logger)
        {
            _roatpDataContext = roatpDataContext;
            _logger = logger;
        }

        public async Task<bool> LoadProvider(Provider provider)
        {
            try
            {
                await _roatpDataContext.Providers.AddAsync(provider);
                await _roatpDataContext.SaveChangesAsync();
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Provider {ukprn} load failed on database update", provider.Ukprn);
                _roatpDataContext.Providers.Remove(provider);
                return false;
            }

            return true;
        }
    }
}