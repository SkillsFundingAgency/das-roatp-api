using MediatR;
using SFA.DAS.Roatp.Application.Common;

namespace SFA.DAS.Roatp.Application.ProviderCourseLocations.Queries.GetProviderCourseLocations
{
    public class GetProviderCourseLocationsQuery : IRequest<GetProviderCourseLocationsQueryResult>, ILarsCode, IUkprn
    {
        public int Ukprn { get; }
        public int LarsCode { get; }

        public GetProviderCourseLocationsQuery(int ukprn, int larsCode)
        {
            Ukprn = ukprn;
            LarsCode = larsCode;
        }
    }
}
