using SFA.DAS.Roatp.Domain.Entities;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Domain.Interfaces
{
    public interface IProviderCourseLocationsWriteRepository
    {
        Task<ProviderCourseLocation> Create(ProviderCourseLocation providerCourseLocation);
        Task Delete(Guid navigationId);
    }
}
