using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Application.Mediatr.Responses;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.ProviderCourse.Queries.GetProviderCourse
{
    public class GetProviderCourseQueryHandler : IRequestHandler<GetProviderCourseQuery, ValidatedResponse<ProviderCourseModel>>
    {
        private readonly IProviderCoursesReadRepository _providerCoursesReadRepository;
        private readonly IStandardsReadRepository _standardsReadRepository;
        private readonly ILogger<GetProviderCourseQueryHandler> _logger;

        public GetProviderCourseQueryHandler(IProviderCoursesReadRepository providerCoursesReadRepository, IStandardsReadRepository standardsReadRepository, ILogger<GetProviderCourseQueryHandler> logger)
        {
            _providerCoursesReadRepository = providerCoursesReadRepository;
            _standardsReadRepository = standardsReadRepository;
            _logger = logger;
        }

        public async Task<ValidatedResponse<ProviderCourseModel>> Handle(GetProviderCourseQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Getting course for {Ukprn} larscode {LarsCode}", request.Ukprn, request.LarsCode);
            ProviderCourseModel providerCourse = await _providerCoursesReadRepository.GetProviderCourseByUkprn(request.Ukprn, request.LarsCode);
            var standardLookup = await _standardsReadRepository.GetStandard(request.LarsCode);
            providerCourse.AttachCourseDetails(standardLookup.IfateReferenceNumber, standardLookup.Level, standardLookup.Title, standardLookup.Version, standardLookup.ApprovalBody);
            return new ValidatedResponse<ProviderCourseModel>(providerCourse);
        }
    }
}
