using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Data.Repositories;

[ExcludeFromCodeCoverage]
internal class NationalAchievementRatesOverallImportWriteRepository : INationalAchievementRatesOverallImportWriteRepository
{
    private readonly RoatpDataContext _roatpDataContext;
    private readonly ILogger<NationalAchievementRatesOverallImportWriteRepository> _logger;

    public NationalAchievementRatesOverallImportWriteRepository(RoatpDataContext roatpDataContext, ILogger<NationalAchievementRatesOverallImportWriteRepository> logger)
    {
        _roatpDataContext = roatpDataContext;
        _logger = logger;
    }

    public async Task Reload(List<NationalAchievementRateOverallImport> items)
    {
        var strategy = _roatpDataContext.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await _roatpDataContext.Database.BeginTransactionAsync();
            try
            {
                await _roatpDataContext.Database.ExecuteSqlInterpolatedAsync($"DELETE FROM NationalAchievementRateOverallImport");
                await _roatpDataContext.BulkInsertAsync(items);
                await _roatpDataContext.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "NationalAchievementRateOverallImport reload failed on database update");
                throw;
            }
        });
    }
}
