using MediatR;
using SFA.DAS.Roatp.Application.Common;

namespace SFA.DAS.Roatp.Application.Courses.Queries.GetProviderDetailsForCourse;

public class GetProviderDetailsForCourseQuery : IRequest<GetProviderDetailsForCourseQueryResult>, IUkprn, ILarsCodeUkprn, ILatLon
{
    public int LarsCode { get; }
    public int Ukprn { get; }
    public double? Latitude { get; }
    public double? Longitude { get; }
    public GetProviderDetailsForCourseQuery(int larsCode, int ukprn,  double? latitude, double? longitude)
    {
        LarsCode = larsCode;
        Ukprn = ukprn;
        Latitude = latitude;
        Longitude = longitude;
    }
}