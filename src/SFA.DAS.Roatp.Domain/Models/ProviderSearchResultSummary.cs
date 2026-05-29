namespace SFA.DAS.Roatp.Domain.Models;

public class ProviderSearchResultSummary
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public int TotalCount { get; set; }
    public string LarsCode { get; set; }
    public string StandardName { get; set; }
    public CourseType CourseType { get; set; }
    public LearningType LearningType { get; set; }
    public bool IsActiveAvailable { get; set; }
    public string QarPeriod { get; set; }
    public string ReviewPeriod { get; set; }
}
