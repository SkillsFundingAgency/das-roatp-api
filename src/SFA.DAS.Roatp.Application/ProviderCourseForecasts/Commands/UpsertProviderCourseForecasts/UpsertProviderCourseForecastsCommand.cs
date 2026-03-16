using System.Collections.Generic;
using MediatR;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Application.Mediatr.Responses;

namespace SFA.DAS.Roatp.Application.ProviderCourseForecasts.Commands.UpsertProviderCourseForecasts;

public class UpsertProviderCourseForecastsCommand : ILarsCodeUkprn, IRequest<ValidatedResponse>
{
    public int Ukprn { get; set; }
    public string LarsCode { get; set; }
    public IEnumerable<UpsertProviderCourseForecastModel> Forecasts { get; set; }
    public UpsertProviderCourseForecastsCommand() { }
    public UpsertProviderCourseForecastsCommand(int ukprn, string larsCode, IEnumerable<UpsertProviderCourseForecastModel> forecasts)
    {
        Ukprn = ukprn;
        LarsCode = larsCode;
        Forecasts = forecasts;
    }
}
