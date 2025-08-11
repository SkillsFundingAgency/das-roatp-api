using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Application.Mediatr.Responses;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.ProviderContact.Commands.CreateProviderContact;

public class CreateProviderContactCommandHandler(IProvidersReadRepository _providersReadRepository, IProviderContactsWriteRepository _providerContactsWriteRepository, ILogger<CreateProviderContactCommandHandler> _logger) : IRequestHandler<CreateProviderContactCommand, ValidatedResponse<long>>
{
    public async Task<ValidatedResponse<long>> Handle(CreateProviderContactCommand command, CancellationToken cancellationToken)
    {
        var provider = await _providersReadRepository.GetByUkprn(command.Ukprn);

        _logger.LogInformation("Adding contact to provider: {Ukprn} with id:{Id}", command.Ukprn, provider.Id);

        Domain.Entities.ProviderContact providerContact = command;
        providerContact.ProviderId = provider.Id;
        providerContact.CreatedDate = DateTime.UtcNow;

        await _providerContactsWriteRepository.CreateProviderContact(providerContact, command.Ukprn,
            command.UserId, command.UserDisplayName, command.ProviderCourseIds);

        _logger.LogInformation("Added contact for provider: {Ukprn} provider contact id {Id}", command.Ukprn, providerContact.Id);
        return new ValidatedResponse<long>(providerContact.Id);
    }
}
