using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Domain.Interfaces;

public interface IRestrictedCourseWriteRepository
{
    Task CreateRestrictedCourse(string larsCode, string userId, string userDisplayName, string userAction);
}