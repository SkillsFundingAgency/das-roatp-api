using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Domain.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Application.Standards.Queries
{
    public class GetAllStandardsQueryHandler : IRequestHandler<GetAllStandardsQuery, GetAllStandardsQueryResult>
    {
        private readonly IStandardsReadRepository _standardsReadRepository;
        private readonly ILogger<GetAllStandardsQueryHandler> _logger;

        public GetAllStandardsQueryHandler(IStandardsReadRepository standardsReadRepository, ILogger<GetAllStandardsQueryHandler> logger)
        {
            _standardsReadRepository = standardsReadRepository;
            _logger = logger;
        }

        public async Task<GetAllStandardsQueryResult> Handle(GetAllStandardsQuery request, CancellationToken cancellationToken)
        {
            var allStandards = await _standardsReadRepository.GetAllStandards();
            _logger.LogInformation($"Returning {allStandards.Count} standards");
            return new GetAllStandardsQueryResult(allStandards);
        }
    }
}
