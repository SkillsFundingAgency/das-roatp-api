using MediatR;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Application.Mediatr.Responses;

namespace SFA.DAS.Roatp.Application.ProviderCourseForecasts.Queries.GetProviderCourseForecasts;

public record GetProviderCourseForecastsQuery(int Ukprn, string LarsCode) : ILarsCodeUkprn, IRequest<ValidatedResponse<GetProviderCourseForecastsQueryResult>>;
