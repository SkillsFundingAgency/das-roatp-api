using SFA.DAS.Roatp.Domain.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Domain.Interfaces;

public interface ITrainingCoursesReadRepository
{
    Task<List<CourseInformation>> GetProviderTrainingCourses(int[] larsCodes, decimal? longitude, decimal? latitude, int? distance, CancellationToken cancellationToken);
}
