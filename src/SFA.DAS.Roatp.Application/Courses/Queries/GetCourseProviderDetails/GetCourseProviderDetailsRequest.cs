using System;

namespace SFA.DAS.Roatp.Application.Courses.Queries.GetCourseProviderDetails;

public sealed class GetCourseProviderDetailsRequest
{
    public decimal? Longitude { get; set; }
    public decimal? Latitude { get; set; }
    public string Location { get; set; }
    public Guid ShortlistUserId { get; set; }
}
