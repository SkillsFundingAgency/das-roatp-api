using MediatR;

namespace SFA.DAS.Roatp.Application.ProviderCourse.Queries.GetProvidersCountForCourse
{
    public class GetProvidersCountForCourseQuery : IRequest<GetProvidersCountForCourseQueryResult>
    {
        public int LarsCode { get; }

        public GetProvidersCountForCourseQuery(int larsCode)
        {
            LarsCode = larsCode;
        }
    }
}
