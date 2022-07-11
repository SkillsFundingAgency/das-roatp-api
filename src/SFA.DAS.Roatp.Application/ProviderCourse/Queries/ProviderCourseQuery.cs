using MediatR;

namespace SFA.DAS.Roatp.Application.ProviderCourse.Queries
{
    public class ProviderCourseQuery : IRequest<ProviderCourseQueryResult>
    {
        public int Ukprn { get; }
        public int LarsCode { get; }

        public ProviderCourseQuery(int ukprn, int larsCode)
        {
            Ukprn = ukprn;
            LarsCode = larsCode;
        }

        public ProviderCourseQuery(){}
    }
}
