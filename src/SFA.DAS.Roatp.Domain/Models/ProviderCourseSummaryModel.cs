namespace SFA.DAS.Roatp.Domain.Models;

public class ProviderCourseSummaryModel
{
    public int ProviderId { get; set; }
    public int Ukprn { get; set; }
    public string LegalName { get; set; }
    public string TradingName { get; set; }
    public double? Distance { get; set; }
}