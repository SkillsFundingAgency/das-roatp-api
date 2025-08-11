using System.Collections.Generic;
using MediatR;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Application.Mediatr.Responses;

namespace SFA.DAS.Roatp.Application.ProviderContact.Commands.CreateProviderContact;

public class CreateProviderContactCommand : IRequest<ValidatedResponse<long>>, IUkprn, IUserInfo
{
    public string EmailAddress { get; set; }
    public string PhoneNumber { get; set; }
    public string UserId { get; set; }
    public string UserDisplayName { get; set; }
    public int Ukprn { get; set; }
    public List<int> ProviderCourseIds { get; set; } = new List<int>();

    public static implicit operator Domain.Entities.ProviderContact(CreateProviderContactCommand source)
        => new()
        {
            EmailAddress = source.EmailAddress,
            PhoneNumber = source.PhoneNumber
        };
}