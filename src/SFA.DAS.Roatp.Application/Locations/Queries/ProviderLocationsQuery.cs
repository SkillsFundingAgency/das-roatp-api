using MediatR;

namespace SFA.DAS.Roatp.Application.Locations.Queries
{
    public class ProviderLocationsQuery : IRequest<ProviderLocationsQueryResult>
    {
        public int Ukprn { get; }

        public ProviderLocationsQuery(int ukprn) => Ukprn = ukprn;

        public ProviderLocationsQuery() { }
    }
}
