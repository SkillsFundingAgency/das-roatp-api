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
    internal class NationalAchievementRatesWriteRepository : INationalAchievementRatesWriteRepository
    {
        private readonly RoatpDataContext _roatpDataContext;
        private readonly ILogger<NationalAchievementRatesWriteRepository> _logger;

        public NationalAchievementRatesWriteRepository(RoatpDataContext roatpDataContext, ILogger<NationalAchievementRatesWriteRepository> logger)
        {
            _roatpDataContext = roatpDataContext;
            _logger = logger;
        }

        public async Task Reload(List<NationalAchievementRate> items)
        {
            await using var transaction = await _roatpDataContext.Database.BeginTransactionAsync();
            try
            {
                await _roatpDataContext.Database.ExecuteSqlInterpolatedAsync($"DELETE FROM NationalAchievementRate");
                await _roatpDataContext.BulkInsertAsync(items);
                await _roatpDataContext.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "NationalAchievementRate reload failed on database update");
                throw;
            }
        }
    }
}
