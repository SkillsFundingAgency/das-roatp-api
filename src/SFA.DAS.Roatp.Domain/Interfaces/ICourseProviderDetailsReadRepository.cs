using SFA.DAS.Roatp.Domain.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Domain.Interfaces;

public interface ICourseProviderDetailsReadRepository
{
    Task<List<CourseProviderDetailsModel>> GetCourseProviderDetails(GetCourseProviderDetailsParameters parameters, CancellationToken cancellationToken);
}
