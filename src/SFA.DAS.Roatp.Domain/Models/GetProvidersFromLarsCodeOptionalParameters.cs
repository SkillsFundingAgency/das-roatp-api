namespace SFA.DAS.Roatp.Domain.Models;
public class GetProvidersFromLarsCodeOptionalParameters
{
    public int? Page { get; set; }
    public int? PageSize { get; set; }
    public bool? IsWorkplace { get; set; }
    public bool? IsProvider { get; set; }
    public bool? IsBlockRelease { get; set; }
    public bool? IsDayRelease { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public decimal? Distance { get; set; }
    public string QarRange { get; set; }
    public string EmployerProviderRatings { get; set; }
    public string ApprenticeProviderRatings { get; set; }
}
