using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Data.Repositories;

[ExcludeFromCodeCoverage]
internal class ForecastQuartersRepository(RoatpDataContext _context) : IForecastQuartersRepository
{
    public async Task<List<ForecastQuarter>> GetForecastQuarters(CancellationToken cancellationToken)
    {
        return await _context.ForecastQuarters.AsNoTracking().ToListAsync(cancellationToken);
    }
}
