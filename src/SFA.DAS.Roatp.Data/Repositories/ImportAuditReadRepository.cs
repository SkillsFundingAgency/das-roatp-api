using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Data.Repositories
{
    [ExcludeFromCodeCoverage]
    internal class ImportAuditReadRepository : IImportAuditReadRepository
    {
        private readonly RoatpDataContext _roatpDataContext;

        public ImportAuditReadRepository(RoatpDataContext roatpDataContext)
        {
            _roatpDataContext = roatpDataContext;
        }

        public async Task<DateTime> GetLastImportedDateByImportType(ImportType importType)
        {
           return await _roatpDataContext
                .ImportAudits
                .Where(i=>i.ImportType==importType)
                .OrderByDescending(i => i.TimeStarted)
                .Select(i=>i.TimeStarted)
                .FirstOrDefaultAsync();
        }
    }
}