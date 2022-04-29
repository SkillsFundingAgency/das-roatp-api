using System.Collections.Generic;
using MediatR;

namespace SFA.DAS.Roatp.Application.ReloadStandards
{
    public class ReloadStandardsRequest : IRequest<bool>
    {
        public List<Standard> Standards { get; set; }
    }
}
