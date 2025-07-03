using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Application.Mediatr.Responses;
using SFA.DAS.Roatp.Domain.Constants;
using SFA.DAS.Roatp.Domain.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Application.Locations.Commands.DeleteLocation;
public class DeleteProviderLocationCommandHandler : IRequestHandler<DeleteProviderLocationCommand, ValidatedResponse<Unit>>
{
    private readonly IProviderLocationsWriteRepository _providerLocationsWriteRepository;
    private readonly ILogger<DeleteProviderLocationCommandHandler> _logger;

    public DeleteProviderLocationCommandHandler(IProviderLocationsWriteRepository providerLocationsWriteRepository, ILogger<DeleteProviderLocationCommandHandler> logger)
    {
        _providerLocationsWriteRepository = providerLocationsWriteRepository;
        _logger = logger;
    }

    public async Task<ValidatedResponse<Unit>> Handle(DeleteProviderLocationCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting provider location for ukprn: {ukprn}  Id: {id} by user: {userid}", request.Ukprn, request.Id, request.UserId);
        await _providerLocationsWriteRepository.Delete(request.Ukprn, request.Id, request.UserId, request.UserDisplayName, AuditEventTypes.DeleteProviderLocation);
        return new ValidatedResponse<Unit>(Unit.Value);
    }
}


