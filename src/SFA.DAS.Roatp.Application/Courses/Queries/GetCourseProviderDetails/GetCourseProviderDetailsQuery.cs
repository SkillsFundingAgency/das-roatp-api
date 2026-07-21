using System;
using MediatR;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Application.Mediatr.Responses;

namespace SFA.DAS.Roatp.Application.Courses.Queries.GetCourseProviderDetails;

public sealed class GetCourseProviderDetailsQuery : IRequest<ValidatedResponse<GetCourseProviderDetailsQueryResult>>, ICoordinates
{
    public int Ukprn { get; private set; }
    public string LarsCode { get; set; }
    public string LocationName { get; private set; }
    public decimal? Latitude { get; private set; }
    public decimal? Longitude { get; private set; }
    public Guid ShortlistUserId { get; private set; }

    public GetCourseProviderDetailsQuery(int ukprn, string larsCode, Guid shortlistUserId, string locationName = null, decimal? longitude = null, decimal? latitude = null)
    {
        Ukprn = ukprn;
        LarsCode = larsCode;
        ShortlistUserId = shortlistUserId;
        LocationName = string.IsNullOrWhiteSpace(locationName) ? null : locationName.Trim();
        Latitude = latitude;
        Longitude = longitude;
    }
}
