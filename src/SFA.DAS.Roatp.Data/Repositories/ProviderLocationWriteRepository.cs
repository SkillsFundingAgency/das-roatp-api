using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Data.Repositories
{
    [ExcludeFromCodeCoverage]
    internal class ProviderLocationWriteRepository : IProviderLocationWriteRepository
    {
        private readonly RoatpDataContext _roatpDataContext;

        public ProviderLocationWriteRepository(RoatpDataContext roatpDataContext)
        {
            _roatpDataContext = roatpDataContext;
        }

        public async Task<ProviderLocation> Create(ProviderLocation location)
        {
            _roatpDataContext.ProviderLocations.Add(location);
            await _roatpDataContext.SaveChangesAsync();
            return location;
        }
    }
}
