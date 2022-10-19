using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Roatp.Abstractions.Events;
using SFA.DAS.Roatp.Data.Repositories;
using SFA.DAS.Roatp.Domain.Events;
using SFA.DAS.Roatp.Messages.Events;
using SFA.DAS.NServiceBus.Services;

namespace SFA.DAS.Roatp.Domain.EventHandlers
{
    public class ProviderAddedHandler : IDomainEventHandler<ProviderAdded>
    {
        private readonly IEventPublisher _eventPublisher;

        public ProviderAddedHandler(IEventPublisher eventPublisher)
        {
            _eventPublisher = eventPublisher;
        }

        public async Task Handle(ProviderAdded @event, CancellationToken cancellationToken = default)
        {
            var providerId = 1;

            await _eventPublisher.Publish(new ProviderAddedToRoATPEvent(@event.ProviderId));
        }
    }
}
