using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Models;

namespace SFA.DAS.Roatp.Domain.Interfaces;

public interface IStandardsReadRepository
{
    Task<List<Standard>> GetAllStandards();
    Task<Standard> GetStandard(string larsCode);
    Task<int> GetStandardsCount();
    Task<List<Standard>> GetCoursesByCourseType(CourseType courseType, CancellationToken cancellationToken);
}
