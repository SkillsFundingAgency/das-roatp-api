using SFA.DAS.Roatp.Domain.Entities;
using System.Collections.Generic;

namespace SFA.DAS.Roatp.Application.Standards.Queries
{
    public class GetAllStandardsQueryResult
    {
        public List<Standard> Standards { get; }
        public GetAllStandardsQueryResult(List<Standard> standards)
        {
            Standards = standards;
        }
    }
}
