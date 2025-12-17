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
internal class ReloadProviderCourseTypesRepository : IReloadProviderCourseTypesRepository
{
    private readonly RoatpDataContext _roatpDataContext;
    private readonly ILogger<ReloadProviderCourseTypesRepository> _logger;

    public ReloadProviderCourseTypesRepository(RoatpDataContext roatpDataContext, ILogger<ReloadProviderCourseTypesRepository> logger)
    {
        _roatpDataContext = roatpDataContext;
        _logger = logger;
    }

    public async Task<bool> ReloadProviderCourseTypes(List<ProviderCourseType> providerCourseTypes, DateTime timeStarted)
    {
        var strategy = _roatpDataContext.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await _roatpDataContext.Database.BeginTransactionAsync();
            try
            {
                await _roatpDataContext.Database.ExecuteSqlInterpolatedAsync($"truncate table ProviderCourseType");
                await _roatpDataContext.BulkInsertAsync(providerCourseTypes);
                await _roatpDataContext.ImportAudits.AddAsync(new ImportAudit(timeStarted, providerCourseTypes.Count, ImportType.ProviderCourseTypes));
                await _roatpDataContext.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "ProviderCourseTypes reload failed on database update");
                throw;
            }
        });

        return true;
    }
}