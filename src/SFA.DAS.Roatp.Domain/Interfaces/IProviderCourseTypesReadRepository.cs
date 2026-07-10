using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Models;

namespace SFA.DAS.Roatp.Domain.Interfaces;

public interface IProviderCourseTypesReadRepository
{
    Task<List<ProviderCourseType>> GetProviderCourseTypesByUkprn(int ukprn, CancellationToken cancellationToken = default);
    Task<List<int>> GetAllProvidersWithShortCourses(CancellationToken cancellationToken = default);
    Task<List<ProviderCourseType>> GetAllProvidersByCourseType(CourseType courseType, CancellationToken cancellationToken = default);
}