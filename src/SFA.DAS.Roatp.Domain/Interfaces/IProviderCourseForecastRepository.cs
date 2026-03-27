using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Models;

namespace SFA.DAS.Roatp.Domain.Interfaces;

public interface IProviderCourseForecastRepository
{
    Task<List<ProviderCourseForecast>> GetProviderCourseForecasts(int ukprn, string larsCode, CancellationToken cancellationToken);

    Task UpsertProviderCourseForecasts(IEnumerable<ProviderCourseForecast> forecasts, CancellationToken cancellationToken);

    Task<List<ProviderCourseWithLastForecastDate>> GetProviderCoursesWithRecentForecasts(DateTime cutOffDate, CancellationToken cancellationToken);
}
