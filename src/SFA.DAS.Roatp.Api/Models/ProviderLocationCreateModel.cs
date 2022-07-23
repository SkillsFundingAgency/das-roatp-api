using SFA.DAS.Roatp.Application.Locations.Commands.CreateLocation;

namespace SFA.DAS.Roatp.Api.Models
{
    public class ProviderLocationCreateModel
    {
        public string UserId { get; set; }
        public string LocationName { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string Town { get; set; }
        public string Postcode { get; set; }
        public string County { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public string Email { get; set; }
        public string Website { get; set; }
        public string Phone { get; set; }

        public static implicit operator CreateProviderLocationCommand (ProviderLocationCreateModel model)
            => new CreateProviderLocationCommand
            {
                UserId = model.UserId,
                LocationName = model.LocationName,
                AddressLine1 = model.AddressLine1,
                AddressLine2 = model.AddressLine2,
                Town = model.Town,
                Postcode = model.Postcode,
                County = model.County,
                Latitude = model.Latitude,
                Longitude = model.Longitude,
                Email = model.Email,
                Website = model.Website,
                Phone = model.Phone
            };
    }
}
