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
                _logger.LogError(ex, $"Provider {provider.Ukprn} load failed on database update, message: {ex.Message} : {ex.InnerException?.Message}");

                //PRODCHECK
                // if (ex?.InnerException?.Message!=null && ex.InnerException.Message.Contains("UK_ProviderLocation_ProviderId_LocationName"))
                // {
                //     _roatpDataContext.Providers.Remove(provider);
                //     return false;
                // }
                // if (ex?.InnerException?.Message != null && ex.InnerException.Message.Contains("UK_ProviderCourse_ProviderId_LarsCode"))
                // {
                //     _roatpDataContext.Providers.Remove(provider);
                //     return false;
                // }

                _roatpDataContext.Providers.Remove(provider);
                return false;
            }

            return true;
        }
    }
}