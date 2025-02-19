using SFA.DAS.Roatp.Domain.Models;
using System.Collections.Generic;

namespace SFA.DAS.Roatp.Application.Courses.Queries.GetProvidersFromLarsCode;
public class GetProvidersFromLarsCodeRequest
{
    public ProviderOrderBy? OrderBy { get; set; }
    public decimal? Distance { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public List<DeliveryMode?> DeliveryModes { get; set; }

    public List<ProviderRating?> EmployerProviderRatings { get; set; }

    public List<ProviderRating?> ApprenticeProviderRatings { get; set; }
    public List<QarRating?> Qar { get; set; }
    public int? Page { get; set; }
    public int? PageSize { get; set; }
}
