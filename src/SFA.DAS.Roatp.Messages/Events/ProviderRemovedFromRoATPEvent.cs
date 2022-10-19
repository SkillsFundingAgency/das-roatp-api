using System;

namespace SFA.DAS.Roatp.Messages.Events
{
    public class ProviderRemovedFromRoATPEvent
    {
        public ProviderRemovedFromRoATPEvent(int providerId)
        {
            ProviderId = providerId;
        }

        public int ProviderId { get; }
    }
}
