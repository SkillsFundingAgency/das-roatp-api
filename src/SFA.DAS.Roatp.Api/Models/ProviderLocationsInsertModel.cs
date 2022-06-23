using SFA.DAS.Roatp.Application.Locations.Commands.BulkInsert;
using System.Collections.Generic;

namespace SFA.DAS.Roatp.Api.Models
{
    public class ProviderLocationsInsertModel
    {
        public string UserId { get; set; }
        public int LarsCode { get; set; }
        public List<int> SubregionIds { get; set; }

        public static implicit operator BulkInsertProviderLocationsCommand(ProviderLocationsInsertModel model) =>
            new BulkInsertProviderLocationsCommand
            {
                UserId = model.UserId,
                LarsCode = model.LarsCode,
                SubregionIds = model.SubregionIds
            };
    }
}
