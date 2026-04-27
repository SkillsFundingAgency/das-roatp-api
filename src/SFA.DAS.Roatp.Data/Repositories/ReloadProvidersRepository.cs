using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Data.Repositories;

[ExcludeFromCodeCoverage]
internal class ReloadProvidersRepository : IReloadProvidersRepository
{
    private readonly RoatpDataContext _roatpDataContext;
    private readonly ILogger<ReloadProvidersRepository> _logger;

    public ReloadProvidersRepository(RoatpDataContext roatpDataContext, ILogger<ReloadProvidersRepository> logger)
    {
        _roatpDataContext = roatpDataContext;
        _logger = logger;
    }

    public async Task<bool> ReloadProviders(DateTime timeStarted, List<Provider> providers)
    {
        var strategy = _roatpDataContext.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await _roatpDataContext.Database.BeginTransactionAsync();
            try
            {
                await _roatpDataContext.Database.ExecuteSqlInterpolatedAsync($"DELETE FROM Provider");
                await _roatpDataContext.BulkInsertAsync(providers);
                await _roatpDataContext.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Provider reload failed on database update");
                throw;
            }
        });

        return true;
    }
}
