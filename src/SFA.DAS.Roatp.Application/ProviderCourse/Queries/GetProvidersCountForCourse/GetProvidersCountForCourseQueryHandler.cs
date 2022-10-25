using MediatR;
using SFA.DAS.Roatp.Domain.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Application.ProviderCourse.Queries.GetProvidersCountForCourse
{
    public class GetProvidersCountForCourseQueryHandler : IRequestHandler<GetProvidersCountForCourseQuery, GetProvidersCountForCourseQueryResult>
    {
        private readonly IProviderCoursesReadRepository _providerCoursesReadRepository;

        public GetProvidersCountForCourseQueryHandler(IProviderCoursesReadRepository providerCoursesReadRepository)
        {
            _providerCoursesReadRepository = providerCoursesReadRepository;
        }

        public async Task<GetProvidersCountForCourseQueryResult> Handle(GetProvidersCountForCourseQuery request, CancellationToken cancellationToken)
        {
            var result = await _providerCoursesReadRepository.GetProvidersCount(request.LarsCode);
            return new GetProvidersCountForCourseQueryResult { ProvidersCount = result };
        }
    }
}
