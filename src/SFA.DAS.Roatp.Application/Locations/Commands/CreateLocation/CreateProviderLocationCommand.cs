using MediatR;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Application.Mediatr.Responses;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Models;
using System;

namespace SFA.DAS.Roatp.Application.Locations.Commands.CreateLocation
{
    public class CreateProviderLocationCommand : IRequest<ValidatedResponse<int>>, IUkprn, IUserInfo
    {
        public int Ukprn { get; set; }
        public string UserId { get; set; }
        public string UserDisplayName { get; set; }
        public string LocationName { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string Town { get; set; }
        public string Postcode { get; set; }
        public string County { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }

        public static implicit operator ProviderLocation(CreateProviderLocationCommand command)
            => new()
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
                Longitude = command.Longitude
            };
    }
}
