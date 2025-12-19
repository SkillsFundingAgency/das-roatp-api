using System.Collections.Generic;
using SFA.DAS.Roatp.Domain.Entities;

namespace SFA.DAS.Roatp.Application.Standards.Queries.GetAllStandards
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
