using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Data.Repositories
{
    [ExcludeFromCodeCoverage]
    internal class ImportAuditWriteRepository : IImportAuditWriteRepository
    {
        private readonly RoatpDataContext _roatpDataContext;

        public ImportAuditWriteRepository(RoatpDataContext roatpDataContext)
        {
            _roatpDataContext = roatpDataContext;
        }

        public async Task Insert(ImportAudit importAudit)
        {
            _roatpDataContext.ImportAudits.Add(importAudit);
                await _roatpDataContext.SaveChangesAsync();
        }
    }
}
