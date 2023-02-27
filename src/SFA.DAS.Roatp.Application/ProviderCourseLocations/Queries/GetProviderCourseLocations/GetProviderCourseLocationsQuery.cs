using MediatR;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Application.Mediatr.Responses;
using System.Collections.Generic;

namespace SFA.DAS.Roatp.Application.ProviderCourseLocations.Queries.GetProviderCourseLocations
{
    public class GetProviderCourseLocationsQuery : IRequest<ValidatedResponse<List<ProviderCourseLocationModel>>>, ILarsCodeUkprn, IUkprn
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
