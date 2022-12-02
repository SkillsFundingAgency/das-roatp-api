using MediatR;
using SFA.DAS.Roatp.Application.Common;

namespace SFA.DAS.Roatp.Application.Courses.Queries.GetProviderDetailsForCourse;

public class GetProvidersForCourseQuery : IRequest<GetProvidersForCourseQueryResult>, ICoordinates, ILarsCode
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