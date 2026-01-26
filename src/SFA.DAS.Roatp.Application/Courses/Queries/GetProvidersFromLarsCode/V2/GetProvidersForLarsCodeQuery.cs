using System;
using System.Collections.Generic;
using System.Linq;
using MediatR;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Application.Courses.Queries.GetProvidersFromLarsCode.V1;
using SFA.DAS.Roatp.Application.Mediatr.Responses;
using SFA.DAS.Roatp.Domain.Models;

namespace SFA.DAS.Roatp.Application.Courses.Queries.GetProvidersFromLarsCode.V2;

public class GetProvidersForLarsCodeQuery : IRequest<ValidatedResponse<GetProvidersForLarsCodeQueryResultV2>>,
    ICoordinates, ILarsCode
{
    public string LarsCode { get; }
    public decimal? Latitude { get; private set; }
    public decimal? Longitude { get; private set; }
    public string Location { get; private set; }
    public ProviderOrderBy? OrderBy { get; private set; }
    public decimal? Distance { get; private set; }
    public int? Page { get; private set; }
    public int? PageSize { get; private set; }
    public Guid? UserId { get; private set; }
    public List<DeliveryModeV2> DeliveryModes { get; } = new();

    public List<ProviderRating> EmployerProviderRatings { get; } = new();

    public List<ProviderRating> ApprenticeProviderRatings { get; } = new();
    public List<QarRating> Qar { get; } = new();

    public GetProvidersForLarsCodeQuery(string larsCode, GetProvidersFromLarsCodeRequestV2 request)
    {
        LarsCode = larsCode;
        PopulateFrom(
            latitude: request.Latitude,
            location: request.Location,
            longitude: request.Longitude,
            orderBy: request.OrderBy,
            distance: request.Distance,
            page: request.Page,
            pageSize: request.PageSize,
            userId: request.UserId,
            deliveryModes: request.DeliveryModes?.Where(x => x != null).Select(x => (DeliveryModeV2)x),
            employerProviderRatings: request.EmployerProviderRatings?.Where(x => x != null).Select(x => (ProviderRating)x),
            apprenticeProviderRatings: request.ApprenticeProviderRatings?.Where(x => x != null).Select(x => (ProviderRating)x),
            qarRatings: request.Qar?.Where(x => x != null).Select(x => (QarRating)x)
        );
    }

    public GetProvidersForLarsCodeQuery(string larsCode, GetProvidersFromLarsCodeRequest request)
    {
        LarsCode = larsCode;
        PopulateFrom(
            latitude: request.Latitude,
            location: request.Location,
            longitude: request.Longitude,
            orderBy: request.OrderBy,
            distance: request.Distance,
            page: request.Page,
            pageSize: request.PageSize,
            userId: request.UserId,
            deliveryModes: request.DeliveryModes?.Where(x => x != null).Select(x => (DeliveryModeV2)x),
            employerProviderRatings: request.EmployerProviderRatings?.Where(x => x != null).Select(x => (ProviderRating)x),
            apprenticeProviderRatings: request.ApprenticeProviderRatings?.Where(x => x != null).Select(x => (ProviderRating)x),
            qarRatings: request.Qar?.Where(x => x != null).Select(x => (QarRating)x)
        );
    }

    private void PopulateFrom(
        decimal? latitude,
        string location,
        decimal? longitude,
        ProviderOrderBy? orderBy,
        decimal? distance,
        int? page,
        int? pageSize,
        Guid? userId,
        IEnumerable<DeliveryModeV2>? deliveryModes,
        IEnumerable<ProviderRating>? employerProviderRatings,
        IEnumerable<ProviderRating>? apprenticeProviderRatings,
        IEnumerable<QarRating>? qarRatings)
    {
        Latitude = latitude;
        Location = location;
        Longitude = longitude;
        OrderBy = orderBy;
        Distance = distance;
        Page = page;
        PageSize = pageSize;
        UserId = userId;

        if (deliveryModes != null) DeliveryModes.AddRange(deliveryModes);
        if (employerProviderRatings != null) EmployerProviderRatings.AddRange(employerProviderRatings);
        if (apprenticeProviderRatings != null) ApprenticeProviderRatings.AddRange(apprenticeProviderRatings);
        if (qarRatings != null) Qar.AddRange(qarRatings);
    }
}