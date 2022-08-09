using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Data.Repositories
{
    [ExcludeFromCodeCoverage]
    internal class ProviderLocationEditRepository : IProviderLocationEditRepository
    {
        private readonly RoatpDataContext _roatpDataContext;

        public ProviderLocationEditRepository(RoatpDataContext context)
        {
            _roatpDataContext = context;
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
