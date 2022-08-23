using System;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Domain.Interfaces
{
    public interface IProviderCourseDeleteRepository
    {
        Task Delete(int ukprn, int larscode);
    }
}
