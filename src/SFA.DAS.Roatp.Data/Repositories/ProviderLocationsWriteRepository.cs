using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Data.Repositories
{
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
            await using var transaction = await _roatpDataContext.Database.BeginTransactionAsync();
            try
            {
                _roatpDataContext.ProviderLocations.Add(location);

                Audit audit = new(typeof(ProviderLocation).Name, ukprn.ToString(), userId, userDisplayName, userAction, location, null);

                _roatpDataContext.Audits.Add(audit);

                await _roatpDataContext.SaveChangesAsync();

                await transaction.CommitAsync();
                return location;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "ProviderLocation create is failed for ukprn {ukprn} by userId {userId}", ukprn, userId);
                throw;
            }
        }

        public async Task UpdateProviderlocation(ProviderLocation updatedProviderLocationEntity)
        {
            var providerLocation = await _roatpDataContext
                .ProviderLocations
                .FindAsync(updatedProviderLocationEntity.Id);

            providerLocation.LocationName = updatedProviderLocationEntity.LocationName;
            providerLocation.Website = updatedProviderLocationEntity.Website;
            providerLocation.Email = updatedProviderLocationEntity.Email;
            providerLocation.Phone = updatedProviderLocationEntity.Phone;

            await _roatpDataContext.SaveChangesAsync();
        }
    }
}
