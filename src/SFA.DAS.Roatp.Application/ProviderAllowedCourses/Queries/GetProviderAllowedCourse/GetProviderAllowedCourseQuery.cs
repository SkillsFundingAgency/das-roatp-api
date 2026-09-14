using MediatR;
using SFA.DAS.Roatp.Application.Common;

namespace SFA.DAS.Roatp.Application.ProviderAllowedCourses.Queries.GetProviderAllowedCourse;

public record GetProviderAllowedCourseQuery(int Ukprn, string LarsCode) : IRequest<GetProviderAllowedCourseQueryResult>, IUkprn, ILarsCode;
