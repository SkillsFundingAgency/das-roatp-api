using System;

namespace SFA.DAS.Roatp.Application.ProviderAllowedCourses.Queries.GetProviderAllowedCourse;

public class GetProviderAllowedCourseQueryResult
{
    public DateTime? LastDateStarts { get; set; }
    public bool IsCourseRestricted { get; set; }
    public bool IsClosedToNewStarts { get; set; }
    public bool IsActive { get; set; }
}
