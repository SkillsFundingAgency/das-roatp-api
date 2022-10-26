using MediatR;

namespace SFA.DAS.Roatp.Application.ProviderCourse.Queries.GetProvidersCountForCourse;

public class GetProviderForCourseQuery : IRequest<GetProviderForCourseQueryResult>
{
    public int LarsCode { get; }
    public int Ukprn { get; }
    public GetProviderForCourseQuery(int larsCode, int ukprn)
    {
        LarsCode = larsCode;
        Ukprn = ukprn;
    }
}