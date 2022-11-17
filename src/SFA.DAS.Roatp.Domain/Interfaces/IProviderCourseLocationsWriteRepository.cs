using SFA.DAS.Roatp.Domain.Entities;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Domain.Interfaces
{
    public interface IProviderCourseLocationsWriteRepository
    {
        Task<ProviderCourseLocation> Create(ProviderCourseLocation providerCourseLocation, int ukprn, string userId, string userDisplayName, string userAction);
        Task Delete(Guid navigationId, int ukprn, string userId, string userDisplayName, string userAction);
    }
}
