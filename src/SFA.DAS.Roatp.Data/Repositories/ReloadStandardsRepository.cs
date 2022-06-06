using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Data.Repositories
{
    [ExcludeFromCodeCoverage]
    internal class ReloadStandardsRepository : IReloadStandardsRepository
    {
        private readonly RoatpDataContext _roatpDataContext;
        private readonly ILogger<ReloadStandardsRepository> _logger;

        public ReloadStandardsRepository(RoatpDataContext roatpDataContext, ILogger<ReloadStandardsRepository> logger)
        {
            _roatpDataContext = roatpDataContext;
            _logger = logger;
        }

        public async Task BulkInsert<T>(IList<T> data) where T : class
        {
            using var tx = await _roatpDataContext.Database.BeginTransactionAsync();

            await _roatpDataContext.BulkInsertAsync(data);

            await tx.CommitAsync();
        }

        public async Task<bool> ReloadStandards(List<Standard> standards)
        {
            await using var transaction = await _roatpDataContext.Database.BeginTransactionAsync();
            try
            {
                await _roatpDataContext.Database.ExecuteSqlInterpolatedAsync($"DELETE FROM Standard");
                await _roatpDataContext.BulkInsertAsync(standards);
                await _roatpDataContext.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Standards reload failed on database update");
                throw;
            }

            return true;
        }
    }
}
