using Microsoft.EntityFrameworkCore;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Data.Repositories
{
    [ExcludeFromCodeCoverage]
    internal class ProvidersWriteRepository : IProvidersWriteRepository
    {
        private readonly RoatpDataContext _roatpDataContext;

        public ProvidersWriteRepository(RoatpDataContext roatpDataContext)
        {
            _roatpDataContext = roatpDataContext;
        }

        public async Task Create(Provider provider)
        {
            _roatpDataContext.Providers.Add(provider);
            await _roatpDataContext.SaveChangesAsync();
        }

        public async Task<List<Provider>> GetAllProviders()
        {
            return await _roatpDataContext.Providers
                        .Include(p => p.ProviderAddress)
                        .ToListAsync();
        }
    }
}
