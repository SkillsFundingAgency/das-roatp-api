using MediatR;

namespace SFA.DAS.Roatp.Application.Region.Queries
{
    public class RegionsQuery : IRequest<RegionsQueryResult>
    {
        public int Ukprn { get; }

        public RegionsQuery(int ukprn) => Ukprn = ukprn;

        public RegionsQuery() { }
    }
}
