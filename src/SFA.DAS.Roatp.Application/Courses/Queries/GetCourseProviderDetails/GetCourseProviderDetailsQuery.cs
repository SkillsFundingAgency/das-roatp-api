using MediatR;
using SFA.DAS.Roatp.Application.Mediatr.Responses;
using System;

namespace SFA.DAS.Roatp.Application.Courses.Queries.GetCourseProviderDetails;

public sealed class GetCourseProviderDetailsQuery : IRequest<ValidatedResponse<GetCourseProviderDetailsQueryResult>>
{
    public int Ukprn { get; set; }
    public int LarsCode { get; set; }
    public decimal? Lon { get; set; }
    public decimal? Lat { get; set; }
    public string Location { get; set; }
    public Guid UserId { get; set; }
}
