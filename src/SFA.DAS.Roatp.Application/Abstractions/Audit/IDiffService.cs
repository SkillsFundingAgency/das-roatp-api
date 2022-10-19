using System.Collections.Generic;

namespace SFA.DAS.Roatp.Abstractions.Audit
{
    public interface IDiffService
    {
        IReadOnlyList<DiffItem> GenerateDiff(Dictionary<string, object> initial, Dictionary<string, object> updated);
    }
}
