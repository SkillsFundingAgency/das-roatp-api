using System.Collections.Generic;
using MediatR;
using SFA.DAS.Roatp.Application.Mediatr.Responses;

namespace SFA.DAS.Roatp.Application.Locations.Queries.GetProviderLocations
{
    public class GetProviderLocationsQuery : IRequest<ValidatedResponse<List<ProviderLocationModel>>>
    {
        public int Ukprn { get; }

        public GetProviderLocationsQuery(int ukprn) => Ukprn = ukprn;
    }
}
