using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Data.Repositories
{
    [ExcludeFromCodeCoverage]
    public class ProviderCourseLocationsWriteRepository : IProviderCourseLocationsWriteRepository
    {
        private readonly RoatpDataContext _roatpDataContext;
        private readonly ILogger<ProviderCourseLocationsWriteRepository> _logger;

        public ProviderCourseLocationsWriteRepository(RoatpDataContext roatpDataContext, ILogger<ProviderCourseLocationsWriteRepository> logger)
        {
            _roatpDataContext = roatpDataContext;
            _logger = logger;
        }

        public async Task<ProviderCourseLocation> Create(ProviderCourseLocation providerCourseLocation)
        {
            _roatpDataContext.ProviderCoursesLocations.Add(providerCourseLocation);
            await _roatpDataContext.SaveChangesAsync();
            return providerCourseLocation;
        }

        public async Task Delete(Guid navigationId, int ukprn, string userId, string userDisplayName, string userAction)
        {
            await using var transaction = await _roatpDataContext.Database.BeginTransactionAsync();
            try
            {
                var location = await _roatpDataContext.ProviderCoursesLocations
                .Where(l => l.NavigationId == navigationId)
                .SingleAsync();

                Audit audit = new(typeof(ProviderCourseLocation).Name, location.Id.ToString(), userId, userDisplayName, userAction, location, null);

                _roatpDataContext.Audits.Add(audit);

                _roatpDataContext.Remove(location);

                await _roatpDataContext.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "ProviderCourseLocation delete is failed for ukprn {ukprn} navigationId {navigationId} by userId {userId}", ukprn, navigationId, userId);
                throw;
            }
        }
    }
}
