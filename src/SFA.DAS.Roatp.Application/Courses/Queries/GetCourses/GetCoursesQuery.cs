using MediatR;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Application.Mediatr.Responses;
using System;

namespace SFA.DAS.Roatp.Application.Courses.Queries.GetCourses;

public sealed class GetCoursesQuery : IRequest<ValidatedResponse<GetCoursesQueryResult>>, ICoordinates
{
    public int[] LarsCodes { get; set; } = Array.Empty<int>();

    public int? Distance { get; set; }

    public decimal? Latitude { get; set; }

    public decimal? Longitude { get; set; }
}
