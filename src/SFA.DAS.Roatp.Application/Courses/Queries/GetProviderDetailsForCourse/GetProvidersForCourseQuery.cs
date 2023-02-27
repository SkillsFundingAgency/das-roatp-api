using MediatR;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Application.Mediatr.Responses;

namespace SFA.DAS.Roatp.Application.Courses.Queries.GetProviderDetailsForCourse;

public class GetProvidersForCourseQuery : IRequest<ValidatedResponse<GetProvidersForCourseQueryResult>>, ICoordinates, ILarsCode
{
    public int LarsCode { get; }
    public decimal? Latitude { get; }
    public decimal? Longitude { get; }

    public GetProvidersForCourseQuery(int larsCode, decimal? latitude, decimal? longitude)
    {
        LarsCode = larsCode;
        Latitude = latitude;
        Longitude = longitude;
    }
}