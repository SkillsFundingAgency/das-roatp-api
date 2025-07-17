using System;
using MediatR;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Application.Mediatr.Responses;

namespace SFA.DAS.Roatp.Application.Locations.Commands.DeleteLocation;

public class DeleteProviderLocationCommand : IRequest<ValidatedResponse<bool>>, IUkprn, IUserInfo
{
    public int Ukprn { get; set; }
    public Guid Id { get; set; }
    public string UserId { get; set; }
    public string UserDisplayName { get; set; }
    public DeleteProviderLocationCommand(int ukprn, Guid id, string userId, string userDisplayName)
    {
        Ukprn = ukprn;
        Id = id;
        UserId = userId;
        UserDisplayName = userDisplayName;
    }
}