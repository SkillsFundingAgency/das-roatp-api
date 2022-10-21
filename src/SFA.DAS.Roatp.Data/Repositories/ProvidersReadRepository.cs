using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Data.Repositories
{
    [ExcludeFromCodeCoverage]
    internal class ProvidersReadRepository : IProvidersReadRepository
    {
        private readonly RoatpDataContext _roatpDataContext;
        private readonly ILogger<ProvidersReadRepository> _logger;

        public ProvidersReadRepository(RoatpDataContext roatpDataContext, ILogger<ProvidersReadRepository> logger)
        {
            _roatpDataContext = roatpDataContext;
            _logger = logger;
        }
        public async Task<Provider> GetByUkprn(int ukprn)
        {
            try
            {
                return await _roatpDataContext.Providers
                            .Include(p => p.ProviderAddress)
                            .AsNoTracking().SingleOrDefaultAsync(p => p.Ukprn == ukprn);
            }
            catch(Exception e)
            {
                _logger.LogError("Error occured while getting provider {ukprn} error message : {error}", ukprn, e.Message);
                return null;
            }
        }

        public async Task<List<Provider>> GetAllProviders()
        {
            try
            {
                return await _roatpDataContext.Providers
                        .Include(p => p.ProviderAddress)
                        .AsNoTracking().ToListAsync();
            }
            catch (Exception e)
            {
                _logger.LogError("Error occured while getting all providers error message : {error}", e.Message);
               return null;
            }
        }
    }
}
