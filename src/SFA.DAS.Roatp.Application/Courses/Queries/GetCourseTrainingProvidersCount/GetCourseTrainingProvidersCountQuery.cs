﻿using MediatR;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Application.Mediatr.Responses;
using System;

namespace SFA.DAS.Roatp.Application.Courses.Queries.GetCourseTrainingProvidersCount;

public sealed class GetCourseTrainingProvidersCountQuery : IRequest<ValidatedResponse<GetCourseTrainingProvidersCountQueryResult>>, ICoordinates
{
    public int[] LarsCodes { get; set; } = Array.Empty<int>();

    public int? Distance { get; set; }

    public decimal? Latitude { get; set; }

    public decimal? Longitude { get; set; }
}
