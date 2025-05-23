using SFA.DAS.Roatp.Application.Locations.Commands.UpdateProviderLocationDetails;

namespace SFA.DAS.Roatp.Api.Models
{
    public class ProviderLocationEditModel
    {
        public string UserId { get; set; }
        public string UserDisplayName { get; set; }
        public string LocationName { get; set; }

        public static implicit operator UpdateProviderLocationDetailsCommand(ProviderLocationEditModel model) =>
            new()
            {
                UserId = model.UserId,
                UserDisplayName = model.UserDisplayName,
                LocationName = model.LocationName,
            };
    }
}
