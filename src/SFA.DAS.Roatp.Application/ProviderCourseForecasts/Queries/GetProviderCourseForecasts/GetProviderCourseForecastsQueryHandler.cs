using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Roatp.Application.Mediatr.Responses;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Roatp.Domain.Models;

namespace SFA.DAS.Roatp.Application.ProviderCourseForecasts.Queries.GetProviderCourseForecasts;

public class GetProviderCourseForecastsQueryHandler(
    IProviderCourseTypesReadRepository _providerCourseTypesReadRepository,
    IProviderCourseForecastRepository _providerCourseForecastRepository,
    IForecastQuartersRepository _forecastQuartersRepository)
    : IRequestHandler<GetProviderCourseForecastsQuery, ValidatedResponse<GetProviderCourseForecastsQueryResult>>
{
    public async Task<ValidatedResponse<GetProviderCourseForecastsQueryResult>> Handle(GetProviderCourseForecastsQuery request, CancellationToken cancellationToken)
    {
        List<ProviderCourseType> courseTypes = await _providerCourseTypesReadRepository.GetProviderCourseTypesByUkprn(request.Ukprn, cancellationToken);
        if (!courseTypes.Any(ct => ct.CourseType == CourseType.ShortCourse))
        {
            return new ValidatedResponse<GetProviderCourseForecastsQueryResult>((GetProviderCourseForecastsQueryResult)null);
        }

        List<ForecastQuarter> quarters = await _forecastQuartersRepository.GetForecastQuarters(cancellationToken);
        List<ProviderCourseForecast> providerCourseForecasts = await _providerCourseForecastRepository.GetProviderCourseForecasts(request.Ukprn, request.LarsCode, cancellationToken);

        var result = new GetProviderCourseForecastsQueryResult
        {
            Forecasts = quarters.ConvertAll(quarter =>
            {
                var forecast = providerCourseForecasts.Find(q => q.Quarter == quarter.Quarter && q.TimePeriod == quarter.TimePeriod);
                return new ProviderCourseForecastModel
                {
                    TimePeriod = quarter.TimePeriod,
                    Quarter = quarter.Quarter,
                    EstimatedLearners = forecast?.EstimatedLearners,
                    UpdatedDate = forecast?.UpdatedDate,
                    StartDate = quarter.StartDate,
                    EndDate = quarter.EndDate
                };
            })
        };

        return new ValidatedResponse<GetProviderCourseForecastsQueryResult>(result);
    }
}
