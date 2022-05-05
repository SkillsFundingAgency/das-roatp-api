using System.Threading.Tasks;
using SFA.DAS.Roatp.Domain.Entities;

namespace SFA.DAS.Roatp.Domain.Interfaces
{
    public interface IImportCourseDetailsRepository
    {
        public Task<bool> ImportCourseDetails(Provider provider);
    }
}
