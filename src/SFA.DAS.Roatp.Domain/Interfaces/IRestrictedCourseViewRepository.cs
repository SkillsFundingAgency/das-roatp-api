using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Roatp.Domain.Entities;

namespace SFA.DAS.Roatp.Domain.Interfaces;

public interface IRestrictedCourseViewRepository
{
    Task<List<RestrictedCourseView>> GetRestrictedCourses(CancellationToken cancellationToken);
}