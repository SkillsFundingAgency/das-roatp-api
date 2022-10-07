using System;
using System.Collections.Generic;

namespace SFA.DAS.Roatp.Jobs.Requests
{
    public class ProviderAddressLookupRequest
    {
        public List<long> Ukprns { get; set; }
        public DateTime? ProvidersUpdatedSince { get; set; }
    }
}
