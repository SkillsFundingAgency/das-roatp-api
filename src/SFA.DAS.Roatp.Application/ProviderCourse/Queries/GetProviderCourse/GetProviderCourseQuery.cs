using MediatR;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Application.Mediatr.Responses;

namespace SFA.DAS.Roatp.Application.ProviderCourse.Queries.GetProviderCourse
{
    public class GetProviderCourseQuery : IRequest<ValidatedResponse<GetProviderCourseQueryResult>>, ILarsCodeUkprn, IUkprn
    {
        public int Ukprn { get; }
        public int LarsCode { get; }

        public GetProviderCourseQuery(int ukprn, int larsCode)
        {
            Ukprn = ukprn;
            LarsCode = larsCode;
        }
    }
}
