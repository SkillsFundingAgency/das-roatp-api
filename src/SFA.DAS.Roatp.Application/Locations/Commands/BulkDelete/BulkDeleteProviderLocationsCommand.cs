using MediatR;
using System.Collections.Generic;

namespace SFA.DAS.Roatp.Application.Locations.Commands.BulkDelete
{
    public class BulkDeleteProviderLocationsCommand : IRequest<int>
    {
        public int Ukprn { get; set; }
        public int LarsCode { get; set; }
        public string UserId { get; set; }
        public List<int> DeSelectedSubregionIds { get; set; }
    }

}
