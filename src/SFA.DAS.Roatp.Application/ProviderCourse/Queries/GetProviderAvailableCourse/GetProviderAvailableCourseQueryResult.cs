using System;

namespace SFA.DAS.Roatp.Application.ProviderCourse.Queries.GetProviderAvailableCourse;

public class GetProviderAvailableCourseQueryResult()
{
    public string LarsCode { get; set; }
    public int Level { get; set; }
    public string Title { get; set; }
    public string Sector { get; set; }
    public bool IsRegulatedForProvider { get; set; }
    public string ApprovalBody { get; set; }
    public DateTime? LastDateStarts { get; set; }
}
