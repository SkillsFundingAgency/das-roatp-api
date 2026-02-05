using System;
using System.Collections.Generic;
using SFA.DAS.Roatp.Domain.Models;

namespace SFA.DAS.Roatp.Application.Courses.Queries.GetProvidersForLarsCode;

public class GetProvidersForLarsCodeRequest
{
    public ProviderOrderBy? OrderBy { get; set; }
    public decimal? Distance { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public string Location { get; set; }
    public List<DeliveryMode> DeliveryModes { get; set; } = new List<DeliveryMode>();

    public List<ProviderRating> EmployerProviderRatings { get; set; } = new List<ProviderRating>();

    public List<ProviderRating> ApprenticeProviderRatings { get; set; } = new List<ProviderRating>();
    public List<QarRating> Qar { get; set; } = new List<QarRating>();
    public int? Page { get; set; }
    public int? PageSize { get; set; }
    public Guid? UserId { get; set; }
}
