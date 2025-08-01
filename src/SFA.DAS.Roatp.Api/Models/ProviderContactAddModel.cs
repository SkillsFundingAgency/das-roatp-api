using System.Collections.Generic;
using SFA.DAS.Roatp.Application.ProviderContact.Commands.CreateProviderContact;

namespace SFA.DAS.Roatp.Api.Models;

public class ProviderContactAddModel
{
    public string EmailAddress { get; set; }
    public string PhoneNumber { get; set; }
    public string UserId { get; set; } // maybe a guid?
    public string UserDisplayName { get; set; }
    public List<int> ProviderCourseIds { get; set; }

    public static implicit operator CreateProviderContactCommand(ProviderContactAddModel source)
        => new()
        {
            EmailAddress = source.EmailAddress,
            PhoneNumber = source.PhoneNumber,
            UserId = source.UserId,
            UserDisplayName = source.UserDisplayName,
            ProviderCourseIds = source.ProviderCourseIds,
        };
}