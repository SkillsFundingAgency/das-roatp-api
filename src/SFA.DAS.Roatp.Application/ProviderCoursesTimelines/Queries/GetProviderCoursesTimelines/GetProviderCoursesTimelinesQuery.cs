using MediatR;
using SFA.DAS.Roatp.Application.ProviderCoursesTimelines.Queries;

namespace SFA.DAS.Roatp.Application.UnitTests.ProviderCoursesTimelines.Queries.GetProviderCoursesTimelines;

public record GetProviderCoursesTimelinesQuery(int Ukprn) : IRequest<ProviderCoursesTimelineModel>;
