using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Application.Mediatr.Responses;
using SFA.DAS.Roatp.Domain.Constants;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.Locations.Commands.DeleteLocation;
public class DeleteProviderLocationCommandHandler(IProviderLocationsWriteRepository providerLocationsWriteRepository, IProviderLocationsReadRepository providerLocationsReadRepository, ILogger<DeleteProviderLocationCommandHandler> logger) : IRequestHandler<DeleteProviderLocationCommand, ValidatedResponse<bool>>
{
    public async Task<ValidatedResponse<bool>> Handle(DeleteProviderLocationCommand request, CancellationToken cancellationToken)
    {

        var getProviderLocationResponse = await providerLocationsReadRepository.GetProviderLocation(request.Ukprn, request.Id);
        if (getProviderLocationResponse == null)
        {
            logger.LogInformation("Deleting provider location for ukprn: {ukprn}  Id: {id} by user: {userid} has no matching location", request.Ukprn, request.Id, request.UserId);
            return new ValidatedResponse<bool>(false);
        }

        logger.LogInformation("Deleting provider location for ukprn: {ukprn}  Id: {id} by user: {userid}",
            request.Ukprn, request.Id, request.UserId);
        await providerLocationsWriteRepository.Delete(request.Ukprn, request.Id, request.UserId,
            request.UserDisplayName, AuditEventTypes.DeleteProviderLocation);


        return new ValidatedResponse<bool>(true);
    }
}


