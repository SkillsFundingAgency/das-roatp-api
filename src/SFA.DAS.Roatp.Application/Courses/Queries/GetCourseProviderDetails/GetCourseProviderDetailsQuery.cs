using MediatR;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Application.Mediatr.Responses;
using System;

namespace SFA.DAS.Roatp.Application.Courses.Queries.GetCourseProviderDetails;

public sealed class GetCourseProviderDetailsQuery : IRequest<ValidatedResponse<GetCourseProviderDetailsQueryResult>>, ICoordinates
{
    public int Ukprn { get; private set; }
    public int LarsCode { get; private set; }
    public string Location { get; private set; }
    public decimal? Latitude { get; private set; }
    public decimal? Longitude { get; private set; }  
    public Guid ShortlistUserId { get; private set; }

    public GetCourseProviderDetailsQuery(int ukprn, int larsCode, Guid shortlistUserId, string location = null, decimal? longitude = null, decimal? latitude = null)
    {
        Ukprn = ukprn;
        LarsCode = larsCode;
        ShortlistUserId = shortlistUserId;
        Location = string.IsNullOrWhiteSpace(location) ? null : location.Trim();
        Latitude = latitude;
        Longitude = longitude; 
    }
}
