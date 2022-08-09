using MediatR;
using SFA.DAS.Roatp.Application.Common;

namespace SFA.DAS.Roatp.Application.ProviderCourse.Queries.GetProviderCourse
{
    public class GetProviderCourseQuery : IRequest<GetProviderCourseQueryResult>, ILarsCode, IUkprn
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
