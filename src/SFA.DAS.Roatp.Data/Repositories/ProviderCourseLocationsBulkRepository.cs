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
    internal class ProviderCourseLocationsBulkRepository : IProviderCourseLocationsBulkRepository
    {
        private readonly RoatpDataContext _roatpDataContext;
        private readonly ILogger<ProviderCourseLocationsBulkRepository> _logger;

        public ProviderCourseLocationsBulkRepository(RoatpDataContext roatpDataContext, ILogger<ProviderCourseLocationsBulkRepository> logger)
        {
            _roatpDataContext = roatpDataContext;
            _logger = logger;
        }

        public async Task BulkInsert(IEnumerable<ProviderCourseLocation> providerCourseLocations, string userId, string userDisplayName, int ukprn, string userAction)
        {
            await using var transaction = await _roatpDataContext.Database.BeginTransactionAsync();
            try
            {
                await _roatpDataContext.ProviderCoursesLocations.AddRangeAsync(providerCourseLocations);

                Audit audit = new(typeof(ProviderCourseLocation).Name, ukprn.ToString(), userId, userDisplayName, userAction, providerCourseLocations, null);

                _roatpDataContext.Audits.Add(audit);

                await _roatpDataContext.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "ProviderCourseLocation bulk insert failed for ukprn {ukprn} by userId {userId}", ukprn, userId);
                throw;
            }
        }
        public async Task BulkDelete(IEnumerable<int> providerCourseLocationIds, string userId, string userDisplayName, int ukprn, string userAction)
        {
            await using var transaction = await _roatpDataContext.Database.BeginTransactionAsync();
            try
            {
                var providerCourseLocations = await _roatpDataContext.ProviderCoursesLocations
                .Where(l => providerCourseLocationIds.Contains(l.Id))
                .ToListAsync();

                _roatpDataContext.ProviderCoursesLocations.RemoveRange(providerCourseLocations);

                Audit audit = new(typeof(ProviderCourseLocation).Name, ukprn.ToString(), userId, userDisplayName, userAction, providerCourseLocations, null);

                _roatpDataContext.Audits.Add(audit);

                await _roatpDataContext.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "ProviderCourseLocation bulk delete failed for ukprn {ukprn} by userId {userId}", ukprn, userId);
                throw;
            }
        }
    }
}
