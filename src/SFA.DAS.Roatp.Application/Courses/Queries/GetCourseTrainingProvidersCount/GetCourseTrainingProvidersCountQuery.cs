using System;
using MediatR;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Application.Mediatr.Responses;

namespace SFA.DAS.Roatp.Application.Courses.Queries.GetCourseTrainingProvidersCount;

public sealed class GetCourseTrainingProvidersCountQuery : IRequest<ValidatedResponse<GetCourseTrainingProvidersCountQueryResult>>, ICoordinates
{
    public string[] LarsCodes { get; set; } = Array.Empty<string>();

    public int? Distance { get; set; }

    public decimal? Latitude { get; set; }

    public decimal? Longitude { get; set; }
}
