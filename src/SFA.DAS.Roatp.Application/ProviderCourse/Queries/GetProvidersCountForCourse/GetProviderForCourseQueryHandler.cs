using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.ProviderCourse.Queries.GetProvidersCountForCourse;

public class GetProviderForCourseQueryHandler : IRequestHandler<GetProviderForCourseQuery, GetProviderForCourseQueryResult>
{
    private readonly IProviderCoursesReadRepository _providerCoursesReadRepository;

    public GetProviderForCourseQueryHandler(IProviderCoursesReadRepository providerCoursesReadRepository)
    {
        _providerCoursesReadRepository = providerCoursesReadRepository;
    }

    public async Task<GetProviderForCourseQueryResult> Handle(GetProviderForCourseQuery request, CancellationToken cancellationToken)
    {
        // var result = await _providerCoursesReadRepository.GetProvidersCount(request.LarsCode);
        return new GetProviderForCourseQueryResult { Ukprn = request.Ukprn};
    }
}