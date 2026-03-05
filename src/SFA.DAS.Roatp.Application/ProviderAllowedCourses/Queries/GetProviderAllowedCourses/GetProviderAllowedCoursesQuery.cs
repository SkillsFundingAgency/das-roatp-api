using MediatR;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Domain.Models;

namespace SFA.DAS.Roatp.Application.ProviderAllowedCourses.Queries.GetProviderAllowedCourses;

public record GetProviderAllowedCoursesQuery(int Ukprn, CourseType CourseType) : IUkprn, IRequest<GetProviderAllowedCoursesQueryResult>;
