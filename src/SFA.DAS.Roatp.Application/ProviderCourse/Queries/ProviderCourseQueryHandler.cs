using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.ProviderCourse.Queries
{
    public class ProviderCourseQueryHandler : IRequestHandler<ProviderCourseQuery, ProviderCourseQueryResult>
    {
        private readonly IProviderCourseReadRepository _providerCourseReadRepository;
        private readonly IProviderReadRepository _providerReadRepository;
        private readonly IStandardReadRepository _standardReadRepository;
        private readonly ILogger<ProviderCourseQueryHandler> _logger;

        public ProviderCourseQueryHandler(IProviderCourseReadRepository providerCourseReadRepository, IProviderReadRepository providerReadRepository, IStandardReadRepository standardReadRepository, ILogger<ProviderCourseQueryHandler> logger)
        {
            _providerCourseReadRepository = providerCourseReadRepository;
            _providerReadRepository = providerReadRepository;
            _standardReadRepository = standardReadRepository;
            _logger = logger;
        }


        public async Task<ProviderCourseQueryResult> Handle(ProviderCourseQuery request, CancellationToken cancellationToken)
        {
            var provider = await _providerReadRepository.GetByUkprn(request.Ukprn);
            if (provider == null)
            {
                _logger.LogInformation("Provider data not found for {ukprn}", request.Ukprn);
                return null;
            }

            ProviderCourseModel providerCourse = await _providerCourseReadRepository.GetProviderCourse(provider.Id, request.LarsCode);
            if (providerCourse == null)
            {
                _logger.LogInformation("Provider course data not found for {ukprn} and {larsCode}", request.Ukprn, request.LarsCode);
                return null;
            }

            var standardLookup = await _standardReadRepository.GetStandard(request.LarsCode);
            if (standardLookup == null)
            {
                _logger.LogError("Standards Lookup data not found for {ukprn} and {larsCode}", request.Ukprn, request.LarsCode);
                return null;
            }
            providerCourse.UpdateCourseDetails(standardLookup.IfateReferenceNumber, standardLookup.Level, standardLookup.Title, standardLookup.Version, standardLookup.ApprovalBody);

            return new ProviderCourseQueryResult { Course = providerCourse };
        }
    }
}
