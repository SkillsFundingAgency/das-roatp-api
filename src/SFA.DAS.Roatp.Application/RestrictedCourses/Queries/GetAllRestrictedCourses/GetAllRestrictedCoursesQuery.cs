using MediatR;

namespace SFA.DAS.Roatp.Application.RestrictedCourses.Queries.GetAllRestrictedCourses;

public record GetAllRestrictedCoursesQuery(bool Restricted) : IRequest<GetAllRestrictedCoursesQueryResult>;