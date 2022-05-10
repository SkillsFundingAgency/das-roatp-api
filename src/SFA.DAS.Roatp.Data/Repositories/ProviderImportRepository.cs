using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Data.Repositories
{
    public class ProviderImportRepository:IProviderImportRepository
    {
        private readonly RoatpDataContext _roatpDataContext;
        private readonly ILogger<ProviderImportRepository> _logger;
        public ProviderImportRepository(RoatpDataContext roatpDataContext, ILogger<ProviderImportRepository> logger)
        {
            _roatpDataContext = roatpDataContext;
            _logger = logger;
        }

        public async Task<bool> ImportProviderDetails(Provider provider)
        {
            var matchingProvider =
                await _roatpDataContext.Providers.FirstOrDefaultAsync(x => x.Ukprn == provider.Ukprn);


            if (matchingProvider != null)
            {
                _logger.LogError("Provider with ukprn [{ukprn}] is already present in the database",provider.Ukprn);
                return false;
            }

            try
            {
            _roatpDataContext.Providers.Add(provider);
             await _roatpDataContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,"Import provider failed on database insert");
                return false;
            }

            var ukprn = provider.Ukprn;
            _logger.LogInformation("Provider details imported for ukprn {ukprn}", ukprn);
            return true;
        }
    }
}
