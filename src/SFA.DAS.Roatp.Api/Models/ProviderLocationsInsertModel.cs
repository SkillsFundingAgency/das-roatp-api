using System.Collections.Generic;
using SFA.DAS.Roatp.Application.Locations.Commands.BulkInsert;

namespace SFA.DAS.Roatp.Api.Models
{
    public class ProviderLocationsInsertModel
    {
        public string UserId { get; set; }
        public string LarsCode { get; set; }
        public string UserDisplayName { get; set; }
        public List<int> SelectedSubregionIds { get; set; } = new List<int>();

        public static implicit operator BulkInsertProviderLocationsCommand(ProviderLocationsInsertModel model) =>
            new BulkInsertProviderLocationsCommand
            {
                LarsCode = model.LarsCode,
                UserId = model.UserId,
                UserDisplayName = model.UserDisplayName,
                SelectedSubregionIds = model.SelectedSubregionIds
            };
    }
}
