using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Models;

namespace SFA.DAS.Roatp.Domain.Interfaces;

public interface IProviderAllowedCoursesRepository
{
    Task<List<ProviderAllowedCourse>> GetProviderAllowedCourses(int ukprn, CourseType courseType, CancellationToken cancellationToken);
}
