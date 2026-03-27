using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Roatp.Domain.Models;

namespace SFA.DAS.Roatp.Data.Repositories;

internal class ProviderCourseForecastRepository(RoatpDataContext _context) : IProviderCourseForecastRepository
{
    [ExcludeFromCodeCoverage]
    public async Task<List<ProviderCourseForecast>> GetProviderCourseForecasts(int ukprn, string larsCode, CancellationToken cancellationToken)
    {
        return await _context.ProviderCourseForecasts
            .Where(pcf => pcf.Ukprn == ukprn && pcf.LarsCode == larsCode)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task UpsertProviderCourseForecasts(IEnumerable<ProviderCourseForecast> forecasts, CancellationToken cancellationToken)
    {
        foreach (var forecast in forecasts)
        {
            var existingForecast = await _context.ProviderCourseForecasts
                .FirstOrDefaultAsync(pcf => pcf.Ukprn == forecast.Ukprn && pcf.LarsCode == forecast.LarsCode && pcf.TimePeriod == forecast.TimePeriod && pcf.Quarter == forecast.Quarter, cancellationToken);
            if (existingForecast != null)
            {
                existingForecast.EstimatedLearners = forecast.EstimatedLearners;
                existingForecast.UpdatedDate = DateTime.UtcNow;
                _context.ProviderCourseForecasts.Update(existingForecast);
            }
            else
            {
                await _context.ProviderCourseForecasts.AddAsync(forecast, cancellationToken);
            }
        }
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<List<ProviderCourseWithLastForecastDate>> GetProviderCoursesWithRecentForecasts(DateTime cutOffDate, CancellationToken cancellationToken)
    {
        return await _context.ProviderCourseForecasts
            .GroupBy(pcf => new { pcf.Ukprn, pcf.LarsCode })
            .Select(g => new { g.Key.Ukprn, g.Key.LarsCode, LastUpdated = g.Max(pcf => pcf.UpdatedDate) })
            .Where(x => x.LastUpdated > cutOffDate)
            .Select(x => new ProviderCourseWithLastForecastDate(x.Ukprn, x.LarsCode, x.LastUpdated))
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
}
