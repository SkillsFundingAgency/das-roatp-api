using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Application.Mediatr.Responses;
using SFA.DAS.Roatp.Domain.Constants;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.Providers.Commands.CreateProvider
{
    public class CreateProviderCommandHandler : IRequestHandler<CreateProviderCommand, ValidatedResponse<int>>
    {
        private readonly IProvidersWriteRepository _providerWriteRepository;
        private readonly IProviderRegistrationDetailsWriteRepository _providerRegistrationDetailsWriteRepository;
        private readonly ILogger<CreateProviderCommandHandler> _logger;

        public CreateProviderCommandHandler(
            IProvidersWriteRepository providerWriteRepository,
            IProviderRegistrationDetailsWriteRepository providerRegistrationDetailsWriteRepository,
            ILogger<CreateProviderCommandHandler> logger)
        {
            _providerWriteRepository = providerWriteRepository;
            _providerRegistrationDetailsWriteRepository = providerRegistrationDetailsWriteRepository;
            _logger = logger;
        }

        public async Task<ValidatedResponse<int>> Handle(CreateProviderCommand command, CancellationToken cancellationToken)
        {
            Provider provider = command;
            ProviderRegistrationDetail detail = await _providerRegistrationDetailsWriteRepository.GetProviderRegistrationDetail(provider.Ukprn);
            if (detail == null)
            {
                var organisationTypeUnassigned = 0;
                detail = new ProviderRegistrationDetail
                {
                    Ukprn = provider.Ukprn,
                    LegalName = provider.LegalName,
                    StatusId = OrganisationStatus.Onboarding,
                    StatusDate = DateTime.UtcNow,
                    OrganisationTypeId = organisationTypeUnassigned,
                    ProviderTypeId = ProviderType.Main
                };
            }
            provider.ProviderRegistrationDetail = detail;
            await _providerWriteRepository.Create(provider, command.UserId, command.UserDisplayName, AuditEventTypes.CreateProvider);
            _logger.LogInformation("Added provider: {Ukprn}", command.Ukprn);
            return new ValidatedResponse<int>(provider.Id);
        }
    }
}