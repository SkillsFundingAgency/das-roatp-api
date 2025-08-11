using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.Roatp.Domain.Entities;

namespace SFA.DAS.Roatp.Domain.Interfaces;

public interface IProviderContactsWriteRepository
{
    Task<ProviderContact> CreateProviderContact(ProviderContact providerContact, int ukprn, string userId, string userDisplayName, List<int> providerCourseIds);
}