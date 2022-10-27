using MediatR;

namespace SFA.DAS.Roatp.Application.Courses.GetProviderDetailsForCourse;

public class GetProviderDetailsForCourseQuery : IRequest<GetProviderDetailsForCourseQueryResult>
{
    public int LarsCode { get; }
    public int Ukprn { get; }
    public double? Lat { get; }
    public double? Lon { get; }
    public GetProviderDetailsForCourseQuery(int larsCode, int ukprn,  double? lat, double? lon)
    {
        LarsCode = larsCode;
        Ukprn = ukprn;
        Lat = lat;
        Lon = lon;
    }
}