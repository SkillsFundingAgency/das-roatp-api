using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Data.Repositories
{
    internal class CourseReadRepository : ICourseReadRepository
    {
        private readonly RoatpDataContext _roatpDataContext;

        public CourseReadRepository(RoatpDataContext roatpDataContext)
        {
            _roatpDataContext = roatpDataContext;
        }
        public async Task<List<Course>> GetAllCourses()
        {
            return await _roatpDataContext
                .Courses
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
