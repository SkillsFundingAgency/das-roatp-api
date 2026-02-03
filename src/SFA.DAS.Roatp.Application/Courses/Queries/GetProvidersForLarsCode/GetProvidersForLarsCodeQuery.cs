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

    private readonly record struct PopulateParams(
        decimal? Latitude,
        string Location,
        decimal? Longitude,
        ProviderOrderBy? OrderBy,
        decimal? Distance,
        int? Page,
        int? PageSize,
        Guid? UserId,
        IEnumerable<DeliveryMode> DeliveryModes,
        IEnumerable<ProviderRating> EmployerProviderRatings,
        IEnumerable<ProviderRating> ApprenticeProviderRatings,
        IEnumerable<QarRating> QarRatings
    );

    public GetProvidersForLarsCodeQuery(string larsCode, GetProvidersForLarsCodeRequest request)
    {
        LarsCode = larsCode;

        var args = new PopulateParams(
            Latitude: request.Latitude,
            Location: request.Location,
            Longitude: request.Longitude,
            OrderBy: request.OrderBy,
            Distance: request.Distance,
            Page: request.Page,
            PageSize: request.PageSize,
            UserId: request.UserId,
            DeliveryModes: request.DeliveryModes?.OfType<DeliveryMode>(),
            EmployerProviderRatings: request.EmployerProviderRatings?.OfType<ProviderRating>(),
            ApprenticeProviderRatings: request.ApprenticeProviderRatings?.OfType<ProviderRating>(),
            QarRatings: request.Qar?.OfType<QarRating>()
        );

        PopulateFrom(args);
    }

    private void PopulateFrom(PopulateParams args)
    {
        Latitude = args.Latitude;
        Location = args.Location;
        Longitude = args.Longitude;
        OrderBy = args.OrderBy;
        Distance = args.Distance;
        Page = args.Page;
        PageSize = args.PageSize;
        UserId = args.UserId;

        if (args.DeliveryModes != null) DeliveryModes.AddRange(args.DeliveryModes);
        if (args.EmployerProviderRatings != null) EmployerProviderRatings.AddRange(args.EmployerProviderRatings);
        if (args.ApprenticeProviderRatings != null) ApprenticeProviderRatings.AddRange(args.ApprenticeProviderRatings);
        if (args.QarRatings != null) Qar.AddRange(args.QarRatings);
    }
}