using SFA.DAS.Roatp.Application.Locations.Commands.UpdateProviderLocationDetails;

namespace SFA.DAS.Roatp.Api.Models
{
    public class ProviderLocationEditModel
    {
        public string UserId { get; set; }
        public string UserDisplayName { get; set; }
        public string LocationName { get; set; }
        public string Website { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public static implicit operator UpdateProviderLocationDetailsCommand(ProviderLocationEditModel model) =>
            new UpdateProviderLocationDetailsCommand
            {
                UserId = model.UserId,
                UserDisplayName = model.UserDisplayName,
                LocationName = model.LocationName,
                Website = model.Website,
                Email = model.Email,
                Phone = model.Phone,
            };
    }
}
