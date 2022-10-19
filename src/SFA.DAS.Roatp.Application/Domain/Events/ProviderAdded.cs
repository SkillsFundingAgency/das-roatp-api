using System;
using SFA.DAS.Roatp.Abstractions.Events;

namespace SFA.DAS.Roatp.Domain.Events
{
    public class ProviderAdded : IDomainEvent
    {
        public ProviderAdded(int providerId)
        {
            ProviderId = providerId;
        }

        public int ProviderId { get; }
    }
}
