namespace SFA.DAS.Roatp.Domain.Models;

public sealed class ProviderSummaryModel
{
    public int Ukprn { get; set; }
    public string LegalName { get; set; }
    public string TradingName { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public string ContactUrl { get; set; }
    public int ProviderTypeId { get; set; }
    public int StatusId { get; set; }
    public string MarketingInfo { get; set; }
    public bool CanAccessApprenticeshipService { get; set; }
    public string MainAddressLine1 { get; set; }
    public string MainAddressLine2 { get; set; }
    public string MainAddressLine3 { get; set; }
    public string MainAddressLine4 { get; set; }
    public string MainTown { get; set; }
    public string MainPostcode { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public string QARPeriod { get; set; }
    public string Leavers { get; set; }
    public string AchievementRate { get; set; }
    public string NationalAchievementRate { get; set; }
    public string ReviewPeriod { get; set; }
    public string EmployerReviews { get; set; }
    public string EmployerStars { get; set; }
    public string EmployerRating { get; set; }
    public string ApprenticeReviews { get; set; }
    public string ApprenticeStars { get; set; }
    public string ApprenticeRating { get; set; }
}
