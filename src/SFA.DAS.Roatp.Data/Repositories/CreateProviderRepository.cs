using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Data.Repositories
{
    [ExcludeFromCodeCoverage]
    internal class CreateProviderRepository : ICreateProviderRepository
    {
        private readonly RoatpDataContext _roatpDataContext;

        public CreateProviderRepository(RoatpDataContext roatpDataContext)
        {
            _roatpDataContext = roatpDataContext;
        }

        public async Task Create(Provider provider)
        {
            _roatpDataContext.Providers.Add(provider);
            await _roatpDataContext.SaveChangesAsync();
        }
    }
}
