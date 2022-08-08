using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Application.ProviderCourse.Queries.GetProviderAllCourses;
using SFA.DAS.Roatp.Application.ProviderCourse.Queries.GetProviderCourse;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.Provider.Queries.GetProvider
{
    public class GetProviderQueryHandler : IRequestHandler<GetProviderQuery, GetProviderQueryResult>
    {
        private readonly IProviderReadRepository _providerReadRepository;
        private readonly ILogger<GetProviderQueryHandler> _logger;

        public GetProviderQueryHandler(IProviderReadRepository providerReadRepository,  ILogger<GetProviderQueryHandler> logger)
        {
            _providerReadRepository = providerReadRepository;
            _logger = logger;
        }
        
        public async Task<GetProviderQueryResult> Handle(GetProviderQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Getting provider for ukprn [{ukprn}]", request.Ukprn);
            ProviderModel provider = await _providerReadRepository.GetByUkprn(request.Ukprn);
            return new GetProviderQueryResult { Provider = provider };
        }
    }
}
