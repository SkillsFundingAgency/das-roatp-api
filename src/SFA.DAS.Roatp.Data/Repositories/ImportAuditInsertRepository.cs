using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Data.Repositories
{
    [ExcludeFromCodeCoverage]
    internal class ImportAuditInsertRepository : IImportAuditInsertRepository
    {
        private readonly RoatpDataContext _roatpDataContext;

        public ImportAuditInsertRepository(RoatpDataContext roatpDataContext)
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
