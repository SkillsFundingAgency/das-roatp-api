using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Roatp.Domain.Entities;

namespace SFA.DAS.Roatp.Data.Repositories
{
    public class ProviderRepository : IProviderRepository
    {
        private readonly RoatpDataContext _roatpDataContext;

        public ProviderRepository(RoatpDataContext roatpDataContext)
        {
            _roatpDataContext = roatpDataContext;
        }

        public Task<Provider> GetProviderByUkprn(int ukprn)
        {
            return _roatpDataContext.Providers
                .SingleOrDefaultAsync(p => p.Ukprn == ukprn);
        }
    }
}
