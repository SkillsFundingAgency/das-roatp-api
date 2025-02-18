using SFA.DAS.Roatp.Domain.Entities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Domain.Interfaces;

public interface IProvidersCountReadRepository
{
    Task<List<CourseInformation>> GetProviderTrainingCourses(int[] larsCodes, decimal? longitude, decimal? latitude, int? distance, CancellationToken cancellationToken);
}
