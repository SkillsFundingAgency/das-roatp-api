using System.Collections.Generic;
using MediatR;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Application.Mediatr.Responses;

namespace SFA.DAS.Roatp.Application.ProviderCourseLocations.Queries.GetProviderCourseLocations
{
    public class GetProviderCourseLocationsQuery : IRequest<ValidatedResponse<List<ProviderCourseLocationModel>>>, ILarsCodeUkprn, IUkprn
    {
        public int Ukprn { get; }
        public string LarsCode { get; }

        public GetProviderCourseLocationsQuery(int ukprn, string larsCode)
        {
            Ukprn = ukprn;
            LarsCode = larsCode;
        }
    }
}
