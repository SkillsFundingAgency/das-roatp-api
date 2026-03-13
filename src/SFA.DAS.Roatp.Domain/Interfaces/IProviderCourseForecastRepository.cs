using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Roatp.Domain.Entities;

namespace SFA.DAS.Roatp.Domain.Interfaces;

public interface IProviderCourseForecastRepository
{
    Task<List<ProviderCourseForecast>> GetProviderCourseForecasts(int ukprn, string larsCode, CancellationToken cancellationToken);

    Task UpsertProviderCourseForecasts(IEnumerable<ProviderCourseForecast> forecasts, CancellationToken cancellationToken);
}
