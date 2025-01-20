using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Domain.Constants;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.Providers.Commands.PatchProvider
{
    public class PatchProviderCommandHandler : IRequestHandler<PatchProviderCommand>
    {
        private readonly IProvidersWriteRepository _providersWriteRepository;
        private readonly IProvidersReadRepository _providersReadRepository;
        private readonly ILogger<PatchProviderCommandHandler> _logger;

        public PatchProviderCommandHandler(IProvidersWriteRepository providersWriteRepository, IProvidersReadRepository providersReadRepository, ILogger<PatchProviderCommandHandler> logger)
        {
            _providersWriteRepository = providersWriteRepository;
            _providersReadRepository = providersReadRepository;
            _logger = logger;
        }

        public async Task Handle(PatchProviderCommand command, CancellationToken cancellationToken)
        {
            var provider = await
                _providersReadRepository.GetByUkprn(command.Ukprn);

            if (provider == null)
            {
                _logger.LogError("PatchProvider: Provider not found for ukprn: {ukprn}", command.Ukprn);
                throw new InvalidOperationException($"PatchProvider: Provider not found for ukprn: {command.Ukprn}");
            }

            var patchedProvider = (Domain.Models.PatchProvider)provider;

            command.Patch.ApplyTo(patchedProvider);

            provider.MarketingInfo = patchedProvider.MarketingInfo;

            await _providersWriteRepository.Patch(provider, command.UserId, command.UserDisplayName, AuditEventTypes.UpdateProviderDescription);
        }
    }
}
