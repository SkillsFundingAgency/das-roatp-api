using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Data.Repositories
{
    internal class StandardsReloadRepository : IStandardsReloadRepository
    {
        private readonly RoatpDataContext _roatpDataContext;

        public StandardsReloadRepository(RoatpDataContext roatpDataContext)
        {
            _roatpDataContext = roatpDataContext;
        }
        
        public async Task<bool> ReloadStandards(List<Standard> standards)
        {
            var standardsStored = _roatpDataContext.Standards;
           _roatpDataContext.Standards.RemoveRange(standardsStored);
           _roatpDataContext.Standards.AddRange(standards);
           await _roatpDataContext.SaveChangesAsync();
           return true;

        }
    }
}
