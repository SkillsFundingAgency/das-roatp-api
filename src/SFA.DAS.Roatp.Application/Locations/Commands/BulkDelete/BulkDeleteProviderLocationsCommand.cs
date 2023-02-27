using MediatR;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Application.Mediatr.Responses;

namespace SFA.DAS.Roatp.Application.Locations.Commands.BulkDelete
{
    public class BulkDeleteProviderLocationsCommand : IRequest<ValidatedResponse<int>>,  IUkprn, IUserInfo
    {
        public int Ukprn { get; set; }
        public string UserId { get; set; }
        public string UserDisplayName { get; set; }
        public BulkDeleteProviderLocationsCommand(int ukprn, string userId, string userDisplayName)
        {
            Ukprn = ukprn;
            UserId = userId;
            UserDisplayName = userDisplayName;
        }
    }
}
