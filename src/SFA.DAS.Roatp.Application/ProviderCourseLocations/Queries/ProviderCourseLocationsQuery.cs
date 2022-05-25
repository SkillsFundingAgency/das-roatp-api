using MediatR;

namespace SFA.DAS.Roatp.Application.ProviderCourseLocations.Queries
{
    public class ProviderCourseLocationsQuery : IRequest<ProviderCourseLocationsQueryResult>
    {
        public int Ukprn { get; }
        public int LarsCode { get; }

        public ProviderCourseLocationsQuery(int ukprn, int larsCode)
        {
            Ukprn = ukprn;
            LarsCode = larsCode;
        }
        public ProviderCourseLocationsQuery() { }
    }
}
