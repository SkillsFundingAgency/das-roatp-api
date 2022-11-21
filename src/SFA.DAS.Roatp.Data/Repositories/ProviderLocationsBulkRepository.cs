using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Data.Repositories
{
    [ExcludeFromCodeCoverage]
    internal class ProviderLocationsBulkRepository :  IProviderLocationsBulkRepository
    {
        private readonly RoatpDataContext _roatpDataContext;
        private readonly ILogger<ProviderLocationsBulkRepository> _logger;
        public ProviderLocationsBulkRepository(RoatpDataContext roatpDataContext, ILogger<ProviderLocationsBulkRepository> logger) 
        {
            _roatpDataContext = roatpDataContext;
            _logger = logger;
        }

        public async Task BulkInsert(IEnumerable<ProviderLocation> providerLocations, string userId, string userDisplayName, int ukprn, string userAction)
        {
            await using var transaction = await _roatpDataContext.Database.BeginTransactionAsync();
            try
            {
                await _roatpDataContext.ProviderLocations.AddRangeAsync(providerLocations);

                Audit audit = new(typeof(ProviderLocation).Name, ukprn.ToString(), userId, userDisplayName, userAction, providerLocations, null);

                _roatpDataContext.Audits.Add(audit);

                await _roatpDataContext.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "ProviderLocation bulk insert failed for ukprn {ukprn} by userId {userId}", ukprn, userId);
                throw;
            }
        }

        public async Task BulkDelete(IEnumerable<int> providerLocationIds, string userId, string userDisplayName, int ukprn, string userAction)
        {
            await using var transaction = await _roatpDataContext.Database.BeginTransactionAsync();
            try
            {
                var providerLocations = await _roatpDataContext.ProviderLocations
                .Where(l => providerLocationIds.Contains(l.Id))
                .ToListAsync();

                Audit audit = new(typeof(ProviderLocation).Name, ukprn.ToString(), userId, userDisplayName, userAction, providerLocations, null);

                _roatpDataContext.Audits.Add(audit);

                _roatpDataContext.ProviderLocations.RemoveRange(providerLocations);

                await _roatpDataContext.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "ProviderLocation bulk delete failed for ukprn {ukprn} by userId {userId}", ukprn, userId);
                throw;
            }
        }
    }
}
