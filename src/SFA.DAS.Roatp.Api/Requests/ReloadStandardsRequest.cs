using System.Collections.Generic;
using MediatR;
using SFA.DAS.Roatp.Api.Models;

namespace SFA.DAS.Roatp.Api.Requests
{
    public class ReloadStandardsRequest: IRequest<bool>
    {
        public List<Standard> Standards { get; set; }
    }
}
