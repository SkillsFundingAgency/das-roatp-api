using SFA.DAS.Roatp.Application.ProviderCourseLocations.Commands.BulkInsert;
using System.Collections.Generic;

namespace SFA.DAS.Roatp.Api.Models
{
    public class ProviderCourseLocationsInsertModel
    {
        public string UserId { get; set; }
        public int LarsCode { get; set; }
        public List<int> SelectedSubregionIds { get; set; }

        public static implicit operator BulkInsertProviderCourseLocationsCommand(ProviderCourseLocationsInsertModel model) =>
            new BulkInsertProviderCourseLocationsCommand
            {
                UserId = model.UserId,
                LarsCode = model.LarsCode,
                SelectedSubregionIds = model.SelectedSubregionIds
            };
    }
}
