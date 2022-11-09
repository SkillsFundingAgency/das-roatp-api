using MediatR;
using SFA.DAS.Roatp.Application.Common;

namespace SFA.DAS.Roatp.Application.Courses.Queries.GetProviderDetailsForCourse;

public class GetAllProviderDetailsForCourseQuery : IRequest<GetAllProviderDetailsForCourseQueryResult>
{
    public int LarsCode { get; }
    public double? Latitude { get; }
    public double? Longitude { get; }
    public short? QuerySortOrder { get; }
    public GetAllProviderDetailsForCourseQuery(int larsCode, double? latitude, double? longitude, short querySortOrder)
    {
        LarsCode = larsCode;
        Latitude = latitude;
        Longitude = longitude;
        QuerySortOrder = querySortOrder;
    }
}