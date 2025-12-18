using MediatR;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Application.Mediatr.Responses;

namespace SFA.DAS.Roatp.Application.ProviderCourseLocations.Commands.AddNationalLocation
{
    public class AddNationalLocationToProviderCourseLocationsCommand : IRequest<ValidatedResponse<int>>, ILarsCodeUkprn, IUkprn, IUserInfo
    {
        public int Ukprn { get; }
        public string LarsCode { get; }
        public string UserId { get; }
        public string UserDisplayName { get; }
        public AddNationalLocationToProviderCourseLocationsCommand(int ukprn, string larsCode, string userId, string userDisplayName)
        {
            Ukprn = ukprn;
            LarsCode = larsCode;
            UserId = userId;
            UserDisplayName = userDisplayName;
        }
    }
}
