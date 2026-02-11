using MediatR;

namespace SFA.DAS.Roatp.Application.ProviderCoursesTimelines.Queries.GetAllProviderCoursesTimelines;

public record GetAllProviderCoursesTimelinesQuery() : IRequest<GetAllProviderCoursesTimelinesQueryResult>;
