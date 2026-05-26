using System.Collections.Generic;
using SFA.DAS.Roatp.Domain.Models;

namespace SFA.DAS.Roatp.Domain.Entities;

public class Standard
{
    public string StandardUId { get; set; }
    public string LarsCode { get; set; }
    public string IfateReferenceNumber { get; set; }
    public int Level { get; set; }
    public string Title { get; set; }
    public string ApprovalBody { get; set; }
    public bool IsRegulatedForProvider { get; set; }
    public int Duration { get; set; }
    public DurationUnits DurationUnits { get; set; }
    public string Route { get; set; }
    public LearningType LearningType { get; set; }
    public CourseType CourseType { get; set; }
    public bool IsActiveAvailable { get; set; }
    public virtual List<ProviderCourse> ProviderCourses { get; set; }
    public virtual List<ProviderCoursesTimeline> ProviderCoursesTimelines { get; set; }
    public virtual List<ProviderAllowedCourse> ProviderAllowedCourses { get; set; }
}
