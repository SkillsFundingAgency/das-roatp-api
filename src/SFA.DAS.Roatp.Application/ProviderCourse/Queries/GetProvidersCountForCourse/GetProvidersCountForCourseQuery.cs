using MediatR;
using SFA.DAS.Roatp.Application.Mediatr.Responses;

namespace SFA.DAS.Roatp.Application.ProviderCourse.Queries.GetProvidersCountForCourse
{
    public class GetProvidersCountForCourseQuery : IRequest<ValidatedResponse<GetProvidersCountForCourseQueryResult>>
    {
        public int LarsCode { get; }

        public GetProvidersCountForCourseQuery(int larsCode)
        {
            LarsCode = larsCode;
        }
    }
}
