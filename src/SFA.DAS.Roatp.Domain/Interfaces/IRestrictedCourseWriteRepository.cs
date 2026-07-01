using System.Threading.Tasks;
using SFA.DAS.Roatp.Domain.Entities;

namespace SFA.DAS.Roatp.Domain.Interfaces;

public interface IRestrictedCourseWriteRepository
{
    Task CreateRestrictedCourse(string larsCode, RestrictedCourse restrictedCourse, string userId, string userDisplayName, string userAction);
}