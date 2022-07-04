using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Data.Repositories
{
    [ExcludeFromCodeCoverage]
    internal class ReloadProviderRegistrationDetailsRepository : IReloadProviderRegistrationDetailsRepository
    {
        private readonly RoatpDataContext _roatpDataContext;
        private readonly ILogger<ReloadProviderRegistrationDetailsRepository> _logger;

        public ReloadProviderRegistrationDetailsRepository(RoatpDataContext roatpDataContext, ILogger<ReloadProviderRegistrationDetailsRepository> logger)
        {
            _roatpDataContext = roatpDataContext;
            _logger = logger;
        }

        public async Task<bool> ReloadRegisteredProviders(List<ProviderRegistrationDetail> providerRegistrationDetails)
        {
            await using var transaction = await _roatpDataContext.Database.BeginTransactionAsync();
            try
            {
                await _roatpDataContext.Database.ExecuteSqlInterpolatedAsync($"DELETE FROM ProviderRegistrationDetail");
                await _roatpDataContext.BulkInsertAsync(providerRegistrationDetails);
                await _roatpDataContext.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "ProviderRegistrationDetail reload failed on database update");
                throw;
            }

            return true;
        }
    }
}
