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
        var strategy = _roatpDataContext.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(async () =>
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
        });

        return true;
    }

    public async Task<bool> UpsertProviderAddresses(List<ProviderAddress> providerAddresses)
    {
        var strategy = _roatpDataContext.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await _roatpDataContext.Database.BeginTransactionAsync();
            try
            {
                foreach (var address in providerAddresses)
                {
                    var providerAddress = await _roatpDataContext.ProviderAddresses.FirstOrDefaultAsync(x => x.ProviderId == address.ProviderId);

                    if (providerAddress != null) _roatpDataContext.Remove(providerAddress);

                    _roatpDataContext.ProviderAddresses.Add(address);
                }

                await _roatpDataContext.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Provider addresses upsert failed on database update");
            }
        });

        return true;
    }
}
