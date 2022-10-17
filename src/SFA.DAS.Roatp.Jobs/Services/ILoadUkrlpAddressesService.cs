using System;
using SFA.DAS.Roatp.Jobs.ApiModels.CourseDirectory;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.Roatp.Domain.Entities;

namespace SFA.DAS.Roatp.Jobs.Services
{
    public interface ILoadUkrlpAddressesService
    { 
        Task<bool> LoadUkrlpAddresses();
        public Task<bool> LoadUkrlpAddressesSinceLastUpdated();
    }
}
