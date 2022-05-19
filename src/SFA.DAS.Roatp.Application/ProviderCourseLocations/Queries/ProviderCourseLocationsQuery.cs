using MediatR;

namespace SFA.DAS.Roatp.Application.ProviderCourseLocations.Queries
{
    public class ProviderCourseLocationsQuery : IRequest<ProviderCourseLocationsQueryResult>
    {
        public int ProviderCourseId { get; }

        public ProviderCourseLocationsQuery(int providerCourseId) => ProviderCourseId = providerCourseId;

        public ProviderCourseLocationsQuery() { }
    }
}
