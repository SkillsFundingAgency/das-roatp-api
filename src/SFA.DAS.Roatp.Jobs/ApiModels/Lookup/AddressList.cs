using System.Collections.Generic;

namespace SFA.DAS.Roatp.Jobs.ApiModels.Lookup;

public class AddressList
{
    public List<LocationAddress> Addresses { get; set; } = new List<LocationAddress>();
}