using MediatR;

namespace SFA.DAS.Roatp.Application.Locations.Queries.GetProviderLocations
{
    public class GetProviderLocationsQuery : IRequest<GetProviderLocationsQueryResult>
    {
        public int Ukprn { get; }

        public GetProviderLocationsQuery(int ukprn) => Ukprn = ukprn;

        public GetProviderLocationsQuery() { }
    }
}
