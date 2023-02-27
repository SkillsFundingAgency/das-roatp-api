using MediatR;
using SFA.DAS.Roatp.Domain.Interfaces;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Roatp.Application.Mediatr.Responses;

namespace SFA.DAS.Roatp.Application.ProviderCourse.Queries.GetProvidersCountForCourse
{
    public class GetProvidersCountForCourseQueryHandler : IRequestHandler<GetProvidersCountForCourseQuery, ValidatedResponse<GetProvidersCountForCourseQueryResult>>
    {
        private readonly IProviderCoursesReadRepository _providerCoursesReadRepository;

        public GetProvidersCountForCourseQueryHandler(IProviderCoursesReadRepository providerCoursesReadRepository)
        {
            _providerCoursesReadRepository = providerCoursesReadRepository;
        }

        public async Task<ValidatedResponse<GetProvidersCountForCourseQueryResult>> Handle(GetProvidersCountForCourseQuery request, CancellationToken cancellationToken)
        {
            var result = await _providerCoursesReadRepository.GetProvidersCount(request.LarsCode);
            return new ValidatedResponse<GetProvidersCountForCourseQueryResult>(new GetProvidersCountForCourseQueryResult { ProvidersCount = result });
        }
    }
}
