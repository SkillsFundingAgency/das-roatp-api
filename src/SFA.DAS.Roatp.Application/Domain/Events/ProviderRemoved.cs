using System;
using SFA.DAS.Roatp.Abstractions.Events;

namespace SFA.DAS.Roatp.Domain.Events
{
    public class ProviderRemoved : IDomainEvent
    {
        public ProviderRemoved(int providerId)
        {
            ProviderId = providerId;
        }

        public int ProviderId { get; }
    }
}
