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

        public ImportAudit GetLastImportByType(string importType)
        {
            return  _roatpDataContext
                    .ImportAudits
                    .AsEnumerable()
                    .OrderByDescending(c => c.TimeStarted)
                    .Where(c => c.ImportType.ToString() == importType).ToList().FirstOrDefault();
        }

        public async Task<ImportAudit> GetLastImportByType(ImportType importType)
        {
           return await _roatpDataContext
                .ImportAudits
                .OrderByDescending(c => c.TimeStarted)
                .FirstOrDefaultAsync(c => c.ImportType == importType);
        }
    }
}