using System.Collections.Generic;

namespace SFA.DAS.Roatp.Abstractions.Audit
{
    public interface IStateService
    {
        Dictionary<string, object> GetState(object item);
    }
}
