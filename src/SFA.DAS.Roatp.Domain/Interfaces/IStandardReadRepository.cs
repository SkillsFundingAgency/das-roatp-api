using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.Roatp.Domain.Entities;

namespace SFA.DAS.Roatp.Domain.Interfaces
{
    public interface IStandardReadRepository
    {
        Task<List<Standard>> GetAllCourses();
        Task<Standard> GetCourse(int larsCode);
    }
}