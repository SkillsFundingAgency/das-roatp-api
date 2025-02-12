using MediatR;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Application.Mediatr.Responses;
using SFA.DAS.Roatp.Domain.Models;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.Roatp.Application.Courses.Queries.GetProvidersFromLarsCode;

public class GetProvidersForLarsCodeQuery : IRequest<ValidatedResponse<GetProvidersForLarsCodeQueryResult>>,
    ICoordinates, ILarsCode
{
    public int LarsCode { get; }
    public ProviderOrderBy? OrderBy { get; }
    public decimal? Distance { get; }
    public decimal? Latitude { get; }
    public decimal? Longitude { get; }
    public List<DeliveryMode> DeliveryModes { get; } = new();

    public List<ProviderRating> EmployerProviderRatings { get; } = new();

    public List<ProviderRating> ApprenticeProviderRatings { get; } = new();
    public List<QarRating> Qar { get; } = new();

    public int? Page { get; }
    public int? PageSize { get; }
    public GetProvidersForLarsCodeQuery(int larsCode, GetProvidersFromLarsCodeRequest request)
    {
        LarsCode = larsCode;
        Latitude = request.Latitude;
        Longitude = request.Longitude;
        OrderBy = request.OrderBy;
        Distance = request.Distance;
        Page = request.Page;
        PageSize = request.PageSize;

        if (request.DeliveryModes != null)
        {
            DeliveryModes.AddRange(from val in request.DeliveryModes where val != null select (DeliveryMode)val);
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