using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Data.Repositories
{
    [ExcludeFromCodeCoverage]
    internal class AuditWriteRepository : IAuditWriteRepository
    {
        private readonly RoatpDataContext _roatpDataContext;

        public AuditWriteRepository(RoatpDataContext roatpDataContext)
        {
            _roatpDataContext = roatpDataContext;
        }

        public async Task Insert(Audit audit)
        {
            _roatpDataContext.Audits.Add(audit);
            await _roatpDataContext.SaveChangesAsync();
        }
    }
}
