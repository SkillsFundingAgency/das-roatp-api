using System;

namespace SFA.DAS.Roatp.Domain.Models;
public class ProviderSearchModel
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public int TotalCount { get; set; }
    public string LarsCode { get; set; }
    public string StandardName { get; set; }
    public string QarPeriod { get; set; }
    public string ReviewPeriod { get; set; }
    public long Ordering { get; set; }
    public int Ukprn { get; set; }
    public string ProviderName { get; set; }
    public int LocationsCount { get; set; }
    public string LocationTypes { get; set; }
    public string CourseDistances { get; set; }
    public string AtEmployers { get; set; }
    public string DayReleases { get; set; }
    public string BlockReleases { get; set; }
    public string Leavers { get; set; }
    public string AchievementRate { get; set; }
    public string EmployerReviews { get; set; }
    public string EmployerStars { get; set; }
    public ProviderRating EmployerRating { get; set; }
    public string ApprenticeReviews { get; set; }
    public string ApprenticeStars { get; set; }
    public ProviderRating ApprenticeRating { get; set; }
    public Guid? ShortlistId { get; set; }
}
