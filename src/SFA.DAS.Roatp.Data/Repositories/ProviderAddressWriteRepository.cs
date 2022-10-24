using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Data.Repositories
{
    [ExcludeFromCodeCoverage]
    public class ProviderAddressWriteRepository: IProviderAddressWriteRepository
    {
        private readonly RoatpDataContext _roatpDataContext;
        private readonly ILogger<ProviderAddressWriteRepository> _logger;

        public ProviderAddressWriteRepository(RoatpDataContext roatpDataContext, ILogger<ProviderAddressWriteRepository> logger)
        {
            _roatpDataContext = roatpDataContext;
            _logger = logger;
        }

        public async Task<bool> Update(ProviderAddress providerAddress)
        {
            try
           {
               _roatpDataContext.ProviderAddresses.Update(providerAddress);
               await _roatpDataContext.SaveChangesAsync();
               return true;
           }
           catch (Exception ex)
           {
               _logger.LogError(ex, "ProviderAddress failed on database update for providerId: {ProviderId}", providerAddress.ProviderId);
               return false;
           }
        }
    }
}
