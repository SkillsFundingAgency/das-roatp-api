using System;

namespace SFA.DAS.Roatp.Domain.Models;

public sealed class CourseProviderDetailsModel
{
    public int Ukprn { get; set; }
    public string ProviderName { get; set; }
    public string MainAddressLine1 { get; set; }
    public string MainAddressLine2 { get; set; }
    public string MainAddressLine3 { get; set; }
    public string MainAddressLine4 { get; set; }
    public string MainTown { get; set; }
    public string MainPostcode { get; set; }
    public string MarketingInfo { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string Website { get; set; }
    public string CourseName { get; set; }
    public int Level { get; set; }
    public string LarsCode { get; set; }
    public string IFateReferenceNumber { get; set; }
    public string Period { get; set; }
    public string Leavers { get; set; }
    public string AchievementRate { get; set; }
    public string NationalLeavers { get; set; }
    public string NationalAchievementRate { get; set; }
    public string ReviewPeriod { get; set; }
    public string EmployerReviews { get; set; }
    public string EmployerStars { get; set; }
    public string EmployerRating { get; set; }
    public string ApprenticeReviews { get; set; }
    public string ApprenticeStars { get; set; }
    public string ApprenticeRating { get; set; }
    public long Ordering { get; set; }
    public bool AtEmployer { get; set; }
    public bool BlockRelease { get; set; }
    public bool DayRelease { get; set; }
    public CourseType CourseType { get; set; }
    public int LocationType { get; set; }
    public string CourseLocation { get; set; }
    public string AddressLine1 { get; set; }
    public string AddressLine2 { get; set; }
    public string Town { get; set; }
    public string County { get; set; }
    public string Postcode { get; set; }
    public double CourseDistance { get; set; }
    public Guid? ShortlistId { get; set; }
}
