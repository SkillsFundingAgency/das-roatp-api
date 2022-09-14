using System.Threading.Tasks;
using SFA.DAS.Roatp.Domain.Entities;

namespace SFA.DAS.Roatp.Domain.Interfaces
{
    public interface IImportAuditReadRepository
    {
        ImportAudit GetLastImportByType(string importType);
        Task<ImportAudit> GetLastImportByType(ImportType importType);
    }
}