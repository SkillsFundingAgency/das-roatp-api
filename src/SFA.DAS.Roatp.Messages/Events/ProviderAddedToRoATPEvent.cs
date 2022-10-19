using System;

namespace SFA.DAS.Roatp.Messages.Events
{
    public class ProviderAddedToRoATPEvent
    {
        public ProviderAddedToRoATPEvent(int providerId)
        {
            ProviderId = providerId;
        }

        public int ProviderId { get; }
    }
}
