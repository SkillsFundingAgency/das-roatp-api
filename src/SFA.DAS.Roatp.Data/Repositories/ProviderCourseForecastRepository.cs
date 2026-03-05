using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Data.Repositories;

[ExcludeFromCodeCoverage]
internal class ProviderCourseForecastRepository(RoatpDataContext _context) : IProviderCourseForecastRepository
{
    public async Task<List<ProviderCourseForecast>> GetProviderCourseForecasts(int ukprn, string larsCode, CancellationToken cancellationToken)
    {
        return await _context.ProviderCourseForecasts
            .Where(pcf => pcf.Ukprn == ukprn && pcf.LarsCode == larsCode)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
}
