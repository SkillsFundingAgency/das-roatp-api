using MediatR;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Domain.Entities;
using System;

namespace SFA.DAS.Roatp.Application.Locations.Commands.CreateLocation
{
    public class CreateProviderLocationCommand : IRequest<Unit>, IUkprn
    {
        public string UserId { get; set; }
        public int Ukprn { get; set; }
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

        public static implicit operator ProviderLocation(CreateProviderLocationCommand command) 
            => new ProviderLocation
            {
                NavigationId = Guid.NewGuid(),
                IsImported = false,
                LocationType = LocationType.Provider,
                LocationName = command.LocationName.Trim(),
                AddressLine1 = command.AddressLine1.Trim(),
                AddressLine2 = command.AddressLine2.Trim(),
                Town = command.Town.Trim(),
                Postcode = command.Postcode.Trim(),
                County = command.County.Trim(),
                Latitude = command.Latitude,
                Longitude = command.Longitude,
                Email = command.Email.Trim(),
                Website = command.Website.Trim(),
                Phone = command.Phone.Trim()
            };
    }
}
