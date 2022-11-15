using MediatR;
using SFA.DAS.Roatp.Application.Common;

namespace SFA.DAS.Roatp.Application.Courses.Queries.GetProviderDetailsForCourse;

public class GetProvidersForCourseQuery : IRequest<GetProvidersForCourseQueryResult>, ILatLon, ILarsCode
{
    public int LarsCode { get; }
    public double? Latitude { get; }
    public double? Longitude { get; }

    public GetProvidersForCourseQuery(int larsCode, double? latitude, double? longitude)
    {
        LarsCode = larsCode;
        Latitude = latitude;
        Longitude = longitude;
    }
}