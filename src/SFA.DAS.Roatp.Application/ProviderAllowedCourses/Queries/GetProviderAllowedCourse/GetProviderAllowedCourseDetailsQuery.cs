using MediatR;
using SFA.DAS.Roatp.Application.Common;

namespace SFA.DAS.Roatp.Application.ProviderAllowedCourses.Queries.GetProviderAllowedCourse;

public record GetProviderAllowedCourseDetailsQuery(int Ukprn, string LarsCode) : IRequest<GetProviderAllowedCourseDetailsQueryResult>, IUkprn, ILarsCode;
