using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Roatp.Domain.Entities;

namespace SFA.DAS.Roatp.Domain.Interfaces;

public interface IProvidersCountReadRepository
{
    Task<List<CourseInformation>> GetProviderTrainingCourses(string[] larsCodes, decimal? longitude, decimal? latitude, int? distance, CancellationToken cancellationToken);
}
