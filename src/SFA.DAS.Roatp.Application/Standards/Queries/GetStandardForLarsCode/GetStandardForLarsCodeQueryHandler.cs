using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Application.Mediatr.Responses;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.Standards.Queries.GetStandardForLarsCode;

public class GetStandardForLarsCodeQueryHandler : IRequestHandler<GetStandardForLarsCodeQuery, ValidatedResponse<GetStandardForLarsCodeQueryResult>>
{
    private readonly IStandardsReadRepository _standardsReadRepository;
    private readonly ILogger<GetStandardForLarsCodeQueryHandler> _logger;

    public GetStandardForLarsCodeQueryHandler(IStandardsReadRepository standardsReadRepository, ILogger<GetStandardForLarsCodeQueryHandler> logger)
    {
        _standardsReadRepository = standardsReadRepository;
        _logger = logger;
    }

    public async Task<ValidatedResponse<GetStandardForLarsCodeQueryResult>> Handle(GetStandardForLarsCodeQuery request, CancellationToken cancellationToken)
    {
        var standard = await _standardsReadRepository.GetStandard(request.LarsCode);
        _logger.LogInformation("Returning standard for larsCode {LarsCode}", request.LarsCode);
        return new ValidatedResponse<GetStandardForLarsCodeQueryResult>((GetStandardForLarsCodeQueryResult)standard);
    }
}