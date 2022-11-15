using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Application.Locations.Commands.CreateLocation
{
    public class CreateProviderLocationCommandHandler : IRequestHandler<CreateProviderLocationCommand, int>
    {
        private readonly IProvidersReadRepository _providersReadRepository;
        private readonly IProviderLocationsWriteRepository _providerLocationsWriteRepository;
        private readonly ILogger<CreateProviderLocationCommandHandler> _logger;

        public CreateProviderLocationCommandHandler(IProvidersReadRepository providersReadRepository, IProviderLocationsWriteRepository providerLocationsWriteRepository, ILogger<CreateProviderLocationCommandHandler> logger)
        {
            _providersReadRepository = providersReadRepository;
            _providerLocationsWriteRepository = providerLocationsWriteRepository;
            _logger = logger;
        }

        public async Task<int> Handle(CreateProviderLocationCommand request, CancellationToken cancellationToken)
        {
            var provider = await _providersReadRepository.GetByUkprn(request.Ukprn);
            _logger.LogInformation("Creating provider location by name {locationName} for ProviderId: {providerId} and Ukprn {ukprn}", request.LocationName, provider.Id, request.Ukprn);
            var providerLocation = (ProviderLocation)request;
            providerLocation.ProviderId = provider.Id;
            var updatedProviderLocation = await _providerLocationsWriteRepository.Create(providerLocation, request.Ukprn, request.UserId, request.UserDisplayName, AuditEventTypes.CreateProviderLocation.ToString());
            return updatedProviderLocation.Id;
        }
    }
}
