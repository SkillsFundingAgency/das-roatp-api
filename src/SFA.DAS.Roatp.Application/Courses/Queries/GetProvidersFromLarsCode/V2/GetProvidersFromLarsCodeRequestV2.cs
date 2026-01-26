using System;
using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Roatp.Application.Courses.Queries.GetProvidersFromLarsCode.V1;
using SFA.DAS.Roatp.Domain.Models;

namespace SFA.DAS.Roatp.Application.Courses.Queries.GetProvidersFromLarsCode.V2;

public class GetProvidersFromLarsCodeRequestV2
{
    public ProviderOrderBy? OrderBy { get; set; }
    public decimal? Distance { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public string Location { get; set; }
    public List<DeliveryModeV2?> DeliveryModes { get; set; }

    public List<ProviderRating?> EmployerProviderRatings { get; set; }

    public List<ProviderRating?> ApprenticeProviderRatings { get; set; }
    public List<QarRating?> Qar { get; set; }
    public int? Page { get; set; }
    public int? PageSize { get; set; }
    public Guid? UserId { get; set; }

    public static implicit operator GetProvidersFromLarsCodeRequestV2(GetProvidersFromLarsCodeRequest v1)
    {
        if (v1 == null) return null;
        return new GetProvidersFromLarsCodeRequestV2
        {
            OrderBy = v1.OrderBy,
            Distance = v1.Distance,
            Latitude = v1.Latitude,
            Longitude = v1.Longitude,
            Location = v1.Location,
            DeliveryModes = v1.DeliveryModes?.Select(dm => (DeliveryModeV2?)dm).ToList(),
            EmployerProviderRatings = v1.EmployerProviderRatings?.Select(r => r).ToList(),
            ApprenticeProviderRatings = v1.ApprenticeProviderRatings?.Select(r => r).ToList(),
            Qar = v1.Qar?.Select(q => q).ToList(),
            Page = v1.Page,
            PageSize = v1.PageSize,
            UserId = v1.UserId
        };
    }
}
