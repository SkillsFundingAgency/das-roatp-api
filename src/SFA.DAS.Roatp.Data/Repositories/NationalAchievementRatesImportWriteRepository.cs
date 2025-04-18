﻿using System;
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
internal class NationalAchievementRatesImportWriteRepository : INationalAchievementRatesImportWriteRepository
{
    private readonly RoatpDataContext _roatpDataContext;
    private readonly ILogger<NationalAchievementRatesImportWriteRepository> _logger;

    public NationalAchievementRatesImportWriteRepository(RoatpDataContext roatpDataContext, ILogger<NationalAchievementRatesImportWriteRepository> logger)
    {
        _roatpDataContext = roatpDataContext;
        _logger = logger;
    }

    public async Task Reload(List<NationalAchievementRateImport> items)
    {
        var strategy = _roatpDataContext.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await _roatpDataContext.Database.BeginTransactionAsync();
            try
            {
                await _roatpDataContext.Database.ExecuteSqlInterpolatedAsync($"DELETE FROM NationalAchievementRateImport");
                await _roatpDataContext.BulkInsertAsync(items);
                await _roatpDataContext.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "NationalAchievementRateImport reload failed on database update");
                throw;
            }
        });
    }
}
