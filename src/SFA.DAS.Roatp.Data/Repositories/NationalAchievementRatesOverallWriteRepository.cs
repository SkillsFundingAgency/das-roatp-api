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
    internal class NationalAchievementRatesOverallWriteRepository : INationalAchievementRatesOverallWriteRepository
    {
        private readonly RoatpDataContext _roatpDataContext;
        private readonly ILogger<NationalAchievementRatesOverallWriteRepository> _logger;

        public NationalAchievementRatesOverallWriteRepository(RoatpDataContext roatpDataContext, ILogger<NationalAchievementRatesOverallWriteRepository> logger)
        {
            _roatpDataContext = roatpDataContext;
            _logger = logger;
        }

        public async Task Reload(List<NationalAchievementRateOverall> items)
        {
            await using var transaction = await _roatpDataContext.Database.BeginTransactionAsync();
            try
            {
                await _roatpDataContext.Database.ExecuteSqlInterpolatedAsync($"DELETE FROM NationalAchievementRateOverall");
                await _roatpDataContext.BulkInsertAsync(items);
                await _roatpDataContext.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "NationalAchievementRateOverall reload failed on database update");
                throw;
            }
        }
    }
}
