using Microsoft.EntityFrameworkCore;
using SFA.DAS.Roatp.Domain.Interfaces;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Data.Repositories;

[ExcludeFromCodeCoverage]
internal class ProviderEmployerStarsReadRepository : IProviderEmployerStarsReadRepository
{
    private readonly RoatpDataContext _roatpDataContext;

    public ProviderEmployerStarsReadRepository(RoatpDataContext roatpDataContext)
    {
        _roatpDataContext = roatpDataContext;
    }

    public async Task<List<string>> GetTimePeriods()
    {
        return await _roatpDataContext.ProviderEmployerStars
            .Select(x => x.TimePeriod)
            .Distinct()
            .ToListAsync();
    }
}