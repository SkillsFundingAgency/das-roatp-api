using SFA.DAS.Roatp.Application.Locations.Commands.BulkDelete;
using System.Collections.Generic;

namespace SFA.DAS.Roatp.Api.Models
{
    public class ProviderLocationsDeleteModel
    {
        public string UserId { get; set; }
        public int LarsCode { get; set; }
        public List<int> DeSelectedSubregionIds { get; set; } = new List<int>();

        public static implicit operator BulkDeleteProviderLocationsCommand(ProviderLocationsDeleteModel model) =>
            new BulkDeleteProviderLocationsCommand
            {
                UserId = model.UserId,
                LarsCode = model.LarsCode,
                DeSelectedSubregionIds = model.DeSelectedSubregionIds
            };
    }
}
