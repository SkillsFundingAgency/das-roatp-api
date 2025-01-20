using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Data.Repositories;

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

    public async Task<ProviderCourseLocation> Create(ProviderCourseLocation providerCourseLocation, int ukprn, string userId, string userDisplayName, string userAction)
    {
        var strategy = _roatpDataContext.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await _roatpDataContext.Database.BeginTransactionAsync();
            try
            {
                _roatpDataContext.ProviderCoursesLocations.Add(providerCourseLocation);

                Audit audit = new(typeof(ProviderCourseLocation).Name, providerCourseLocation.ProviderCourseId.ToString(), userId, userDisplayName, userAction, providerCourseLocation, null);

                _roatpDataContext.Audits.Add(audit);

                await _roatpDataContext.SaveChangesAsync();
                await transaction.CommitAsync();

            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                _logger.LogError(ex, "ProviderCourseLocation create is failed for ukprn {Ukprn} providerCourseId {ProviderCourseId} by userId {UserId}", ukprn, providerCourseLocation.ProviderCourseId, userId);
                throw;
            }
        });
        return providerCourseLocation;
    }

    public async Task Delete(Guid navigationId, int ukprn, string userId, string userDisplayName, string userAction)
    {
        var strategy = _roatpDataContext.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(async () =>
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
                _logger.LogError(ex, "ProviderCourseLocation delete is failed for ukprn {Ukprn} navigationId {NavigationId} by userId {UserId}", ukprn, navigationId, userId);
                throw;
            }
        });
    }
}
