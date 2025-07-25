﻿using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Data.Repositories;

[ExcludeFromCodeCoverage]
internal class ProviderLocationsWriteRepository : IProviderLocationsWriteRepository
{
    private readonly RoatpDataContext _roatpDataContext;
    private readonly ILogger<ProviderLocationsWriteRepository> _logger;
    public ProviderLocationsWriteRepository(RoatpDataContext roatpDataContext, ILogger<ProviderLocationsWriteRepository> logger)
    {
        _roatpDataContext = roatpDataContext;
        _logger = logger;
    }

    public async Task<ProviderLocation> Create(ProviderLocation location, int ukprn, string userId, string userDisplayName, string userAction)
    {
        var strategy = _roatpDataContext.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await _roatpDataContext.Database.BeginTransactionAsync();
            try
            {
                _roatpDataContext.ProviderLocations.Add(location);

                Audit audit = new(typeof(ProviderLocation).Name, ukprn.ToString(), userId, userDisplayName, userAction, location, null);

                _roatpDataContext.Audits.Add(audit);

                await _roatpDataContext.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "ProviderLocation create is failed for ukprn {Ukprn} by userId {UserId}", ukprn, userId);
                throw;
            }
        });
        return location;
    }

    public async Task UpdateProviderlocation(ProviderLocation updatedProviderLocationEntity, int ukprn, string userId, string userDisplayName, string userAction)
    {
        var strategy = _roatpDataContext.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await _roatpDataContext.Database.BeginTransactionAsync();
            try
            {
                var providerLocation = await _roatpDataContext
                .ProviderLocations
                .FindAsync(updatedProviderLocationEntity.Id);

                Audit audit = new(typeof(ProviderLocation).Name, ukprn.ToString(), userId, userDisplayName, userAction, providerLocation, updatedProviderLocationEntity);

                _roatpDataContext.Audits.Add(audit);


                providerLocation!.LocationName = updatedProviderLocationEntity.LocationName;

                await _roatpDataContext.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "ProviderLocation Update is failed for ukprn {Ukprn} by userId {UserId}", ukprn, userId);
                throw;
            }
        });
    }

    public async Task Delete(int ukprn, Guid id, string userId, string userDisplayName, string userAction)
    {
        var strategy = _roatpDataContext.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await _roatpDataContext.Database.BeginTransactionAsync();
            try
            {
                var providerLocation = await _roatpDataContext.ProviderLocations
                .Where(l => l.NavigationId == id)
                .Include(p => p.ProviderCourseLocations)
                .SingleAsync();

                Audit audit = new(nameof(ProviderLocation), providerLocation.Id.ToString(), userId, userDisplayName, userAction, providerLocation, null);

                _roatpDataContext.Audits.Add(audit);

                _roatpDataContext.Remove(providerLocation);

                await _roatpDataContext.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "ProviderLocation delete has failed for ukprn {Ukprn} id {id} by userId {UserId}", ukprn, id, userId);
                throw;
            }
        });
    }
}
