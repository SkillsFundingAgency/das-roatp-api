using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Application.Mediatr.Responses;
using SFA.DAS.Roatp.Domain.Constants;
using SFA.DAS.Roatp.Domain.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Application.Providers.Commands.CreateProvider
{
    public class CreateProviderCommandHandler : IRequestHandler<CreateProviderCommand, ValidatedResponse<int>>
    {
        private readonly IProvidersWriteRepository _providerWriteRepository;

        private readonly ILogger<CreateProviderCommandHandler> _logger;

        public CreateProviderCommandHandler(IProvidersWriteRepository providerWriteRepository, ILogger<CreateProviderCommandHandler> logger)
        {
            _providerWriteRepository = providerWriteRepository;
            _logger = logger;
        }

        public async Task<ValidatedResponse<int>> Handle(CreateProviderCommand command, CancellationToken cancellationToken)
        {
            Domain.Entities.Provider provider = command;
            await _providerWriteRepository.Create(provider,command.UserId,command.UserDisplayName, AuditEventTypes.CreateProvider);
            _logger.LogInformation("Added provider: {ukprn}", command.Ukprn);
            return new ValidatedResponse<int>(provider.Id);
        }
    }
}