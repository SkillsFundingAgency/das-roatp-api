using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Roatp.Application.Mediatr.Responses;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.ProviderCourseForecasts.Commands.UpsertProviderCourseForecasts;

public class UpsertProviderCourseForecastsCommandHandler(
    IProviderCourseForecastRepository _providerCourseForecastRepository,
    IForecastQuartersRepository _forecastQuartersRepository)
    : IRequestHandler<UpsertProviderCourseForecastsCommand, ValidatedResponse>
{
    public async Task<ValidatedResponse> Handle(UpsertProviderCourseForecastsCommand request, CancellationToken cancellationToken)
    {
        IEnumerable<ProviderCourseForecast> validForecasts = [];
        var quarters = await _forecastQuartersRepository.GetForecastQuarters(cancellationToken);
        foreach (var quarter in quarters)
        {
            var matchingForecast = request.Forecasts.FirstOrDefault(f => f.Quarter == quarter.Quarter && f.TimePeriod == quarter.TimePeriod);
            if (matchingForecast is not null)
            {
                validForecasts = validForecasts.Append(new ProviderCourseForecast
                {
                    Ukprn = request.Ukprn,
                    LarsCode = request.LarsCode,
                    TimePeriod = quarter.TimePeriod,
                    Quarter = quarter.Quarter,
                    EstimatedLearners = matchingForecast.EstimatedLearners,
                    CreatedDate = DateTime.UtcNow
                });
            }
        }
        await _providerCourseForecastRepository.UpsertProviderCourseForecasts(validForecasts, cancellationToken);
        return ValidatedResponse.Valid();
    }
}
