using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Data.Repositories;

[ExcludeFromCodeCoverage]
public class ProviderContactReadRepository(RoatpDataContext _roatpDataContext) : IProviderContactsReadRepository
{
    public async Task<ProviderContact> GetLatestProviderContact(int ukprn)
    {
        return await _roatpDataContext.ProviderContacts
            .Include(c => c.Provider)
            .Where(p => p.Provider.Ukprn == ukprn)
            .OrderByDescending(c => c.CreatedDate)
            .FirstOrDefaultAsync();
    }
}
