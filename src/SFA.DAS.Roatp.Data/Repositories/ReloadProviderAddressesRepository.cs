using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;

namespace SFA.DAS.Roatp.Data.Repositories
{
    [ExcludeFromCodeCoverage]
    internal class ReloadProviderAddressesRepository : IReloadProviderAddressesRepository
    {
        private readonly RoatpDataContext _roatpDataContext;
        private readonly ILogger<ReloadProviderAddressesRepository> _logger;

        public ReloadProviderAddressesRepository(RoatpDataContext roatpDataContext, ILogger<ReloadProviderAddressesRepository> logger)
        {
            _roatpDataContext = roatpDataContext;
            _logger = logger;
        }

        public async Task<bool> ReloadProviderAddresses(List<ProviderAddress> providerAddresses)
        {
            await using var transaction = await _roatpDataContext.Database.BeginTransactionAsync();
            try
            {
                await _roatpDataContext.Database.ExecuteSqlInterpolatedAsync($"DELETE FROM ProviderAddress");
                await _roatpDataContext.BulkInsertAsync(providerAddresses);
                await _roatpDataContext.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Provider addresses reload failed on database update");
                throw;
            }

            return true;
        }
    }
}
