using System;
using System.Collections.Generic;
using System.Linq;
using MediatR;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Application.Mediatr.Responses;
using SFA.DAS.Roatp.Domain.Models;

namespace SFA.DAS.Roatp.Application.Courses.Queries.GetProvidersForLarsCode;

public class GetProvidersForLarsCodeQuery : IRequest<ValidatedResponse<GetProvidersForLarsCodeQueryResult>>,
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
    public List<DeliveryMode> DeliveryModes { get; } = new();

    public List<ProviderRating> EmployerProviderRatings { get; } = new();

    public List<ProviderRating> ApprenticeProviderRatings { get; } = new();
    public List<QarRating> Qar { get; } = new();

    public GetProvidersForLarsCodeQuery(string larsCode, GetProvidersForLarsCodeRequest request)
    {
        LarsCode = larsCode;

        Latitude = request.Latitude;
        Location = request.Location;
        Longitude = request.Longitude;
        OrderBy = request.OrderBy;
        Distance = request.Distance;
        Page = request.Page;
        PageSize = request.PageSize;
        UserId = request.UserId;

        var deliveryModes = request.DeliveryModes.OfType<DeliveryMode>();
        if (deliveryModes != null) DeliveryModes.AddRange(deliveryModes);

        var employerRatings = request.EmployerProviderRatings.OfType<ProviderRating>();
        if (employerRatings != null) EmployerProviderRatings.AddRange(employerRatings);

        var apprenticeRatings = request.ApprenticeProviderRatings.OfType<ProviderRating>();
        if (apprenticeRatings != null) ApprenticeProviderRatings.AddRange(apprenticeRatings);

        var qarRatings = request.Qar.OfType<QarRating>();
        if (qarRatings != null) Qar.AddRange(qarRatings);
    }
}