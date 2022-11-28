using MediatR;
using SFA.DAS.Roatp.Application.Common;

namespace SFA.DAS.Roatp.Application.Courses.Queries.GetProviderDetailsForCourse;

public class GetProviderDetailsForCourseQuery : IRequest<GetProviderDetailsForCourseQueryResult>, IUkprn, ILarsCode, ICoordinates

{
    public int LarsCode { get; }
    public int Ukprn { get; }
    public decimal? Latitude { get; }
    public decimal? Longitude { get; }
    public GetProviderDetailsForCourseQuery(int larsCode, int ukprn,  decimal? latitude, decimal? longitude)
    {
        LarsCode = larsCode;
        Ukprn = ukprn;
        Latitude = latitude;
        Longitude = longitude;
    }
}