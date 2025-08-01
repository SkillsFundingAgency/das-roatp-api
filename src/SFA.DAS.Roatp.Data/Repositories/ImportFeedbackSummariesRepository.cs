using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Data.Repositories;

[ExcludeFromCodeCoverage]
internal class ImportFeedbackSummariesRepository(RoatpDataContext _roatpDataContext, ILogger<ImportFeedbackSummariesRepository> _logger) : IImportFeedbackSummariesRepository
{
    public async Task Import(DateTime timeStarted, IEnumerable<ProviderApprenticeStars> providerApprenticeStars, IEnumerable<ProviderEmployerStars> providerEmployerStars)
    {
        var strategy = _roatpDataContext.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(async () =>
        {
            using var transaction = await _roatpDataContext.Database.BeginTransactionAsync();
            try
            {
                await _roatpDataContext.BulkInsertAsync(providerApprenticeStars);
                await _roatpDataContext.BulkInsertAsync(providerEmployerStars);
                _roatpDataContext.ImportAudits.Add(new ImportAudit(timeStarted, providerApprenticeStars.Count(), ImportType.ProviderApprenticeStars));
                _roatpDataContext.ImportAudits.Add(new ImportAudit(timeStarted, providerEmployerStars.Count(), ImportType.ProviderEmployerStars));
                await _roatpDataContext.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to import feedback summaries");
                await transaction.RollbackAsync();
                throw;
            }
        });
    }
}
