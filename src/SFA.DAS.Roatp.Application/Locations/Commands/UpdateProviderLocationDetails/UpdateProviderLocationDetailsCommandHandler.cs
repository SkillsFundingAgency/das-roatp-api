using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Domain.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Application.Locations.Commands.UpdateProviderLocationDetails
{
    public class UpdateProviderLocationDetailsCommandHandler : IRequestHandler<UpdateProviderLocationDetailsCommand, Unit>
    {
        private readonly IProviderLocationsWriteRepository _providerLocationsWriteRepository;
        private readonly IProviderLocationsReadRepository _providerLocationsReadRepository;
        private readonly ILogger<UpdateProviderLocationDetailsCommandHandler> _logger;

        public UpdateProviderLocationDetailsCommandHandler(IProviderLocationsReadRepository providerLocationsReadRepository, IProviderLocationsWriteRepository providerLocationsWriteRepository, ILogger<UpdateProviderLocationDetailsCommandHandler> logger)
        {
            _providerLocationsReadRepository = providerLocationsReadRepository;
            _providerLocationsWriteRepository = providerLocationsWriteRepository;
            _logger = logger;
        }

        public async Task<Unit> Handle(UpdateProviderLocationDetailsCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Update provider location details for ukprn: {ukprn} Id: {id} by user: {userid}", request.Ukprn, request.Id, request.UserId);
            var providerLocation = await _providerLocationsReadRepository.GetProviderLocation(request.Ukprn, request.Id);
            if (providerLocation == null)
            {
                _logger.LogError("Provider location not found for ukprn: {ukprn} Id: {id}", request.Ukprn, request.Id);
                throw new InvalidOperationException($"Provider location not found for ukprn: {request.Ukprn} Id: {request.Id}");
            }

            providerLocation.LocationName = request.LocationName;
            providerLocation.Website = request.Website;
            providerLocation.Email = request.Email;
            providerLocation.Phone = request.Phone;

            await _providerLocationsWriteRepository.UpdateProviderlocation(providerLocation);

            return Unit.Value;
        }
    }
}
