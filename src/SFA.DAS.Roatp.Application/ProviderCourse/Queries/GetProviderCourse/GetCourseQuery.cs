using MediatR;
using SFA.DAS.Roatp.Application.Common;

namespace SFA.DAS.Roatp.Application.ProviderCourse.Queries.GetProviderCourse;

public class GetCourseQuery : IRequest<GetCourseQueryResult>, ILarsCode, IUkprn
{
    public int Ukprn { get; }
    public int LarsCode { get; }

    public GetCourseQuery(int ukprn, int larsCode)
    {
        Ukprn = ukprn;
        LarsCode = larsCode;
    }
}