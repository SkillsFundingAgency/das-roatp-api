using System.Collections.Generic;

namespace SFA.DAS.Roatp.Application.ProviderAllowedCourses.Queries.GetProviderAllowedCourses;

public record GetProviderAllowedCoursesQueryResult(IEnumerable<ProviderAllowedCourseModel> AllowedCourses);
