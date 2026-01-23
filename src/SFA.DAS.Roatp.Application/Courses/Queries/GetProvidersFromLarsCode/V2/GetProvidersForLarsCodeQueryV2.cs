using System;
using System.Collections.Generic;
using System.Linq;
using MediatR;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Application.Courses.Queries.GetProvidersFromLarsCode.V1;
using SFA.DAS.Roatp.Application.Mediatr.Responses;
using SFA.DAS.Roatp.Domain.Models;

namespace SFA.DAS.Roatp.Application.Courses.Queries.GetProvidersFromLarsCode.V2;

public class GetProvidersForLarsCodeQueryV2 : IRequest<ValidatedResponse<GetProvidersForLarsCodeQueryResultV2>>,
    ICoordinates, ILarsCode
{
    public string LarsCode { get; }
    public ProviderOrderBy? OrderBy { get; }
    public decimal? Distance { get; }
    public decimal? Latitude { get; }
    public decimal? Longitude { get; }
    public string Location { get; }
    public List<DeliveryModeV2> DeliveryModes { get; } = new();

    public List<ProviderRating> EmployerProviderRatings { get; } = new();

    public List<ProviderRating> ApprenticeProviderRatings { get; } = new();
    public List<QarRating> Qar { get; } = new();

    public int? Page { get; }
    public int? PageSize { get; }
    public Guid? UserId { get; }
    public GetProvidersForLarsCodeQueryV2(string larsCode, GetProvidersFromLarsCodeRequestV2 request)
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

        if (request.DeliveryModes != null)
        {
            DeliveryModes.AddRange(from val in request.DeliveryModes where val != null select (DeliveryModeV2)val);
        }

        if (request.EmployerProviderRatings != null)
        {
            EmployerProviderRatings.AddRange(from val in request.EmployerProviderRatings where val != null select (ProviderRating)val);
        }

        if (request.ApprenticeProviderRatings != null)
        {
            ApprenticeProviderRatings.AddRange(from val in request.ApprenticeProviderRatings where val != null select (ProviderRating)val);
        }

        if (request.Qar != null)
        {
            Qar.AddRange(from val in request.Qar where val != null select (QarRating)val);
        }
    }

    public GetProvidersForLarsCodeQueryV2(string larsCode, GetProvidersFromLarsCodeRequest request)
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

        if (request.DeliveryModes != null)
        {
            DeliveryModes.AddRange(from val in request.DeliveryModes where val != null select (DeliveryModeV2)val);
        }

        if (request.EmployerProviderRatings != null)
        {
            EmployerProviderRatings.AddRange(from val in request.EmployerProviderRatings where val != null select (ProviderRating)val);
        }

        if (request.ApprenticeProviderRatings != null)
        {
            ApprenticeProviderRatings.AddRange(from val in request.ApprenticeProviderRatings where val != null select (ProviderRating)val);
        }

        if (request.Qar != null)
        {
            Qar.AddRange(from val in request.Qar where val != null select (QarRating)val);
        }
    }
}