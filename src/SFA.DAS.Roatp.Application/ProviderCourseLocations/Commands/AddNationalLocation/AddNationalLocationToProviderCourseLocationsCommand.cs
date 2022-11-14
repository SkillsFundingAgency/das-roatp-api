using MediatR;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Domain.Entities;

namespace SFA.DAS.Roatp.Application.ProviderCourseLocations.Commands.AddNationalLocation
{
    public class AddNationalLocationToProviderCourseLocationsCommand : IRequest<ProviderCourseLocation>, ILarsCodeUkprn, IUkprn
    {
        public int Ukprn { get; }
        public int LarsCode { get; }
        public string UserId { get; }
        public string UserDisplayName { get; }
        public AddNationalLocationToProviderCourseLocationsCommand(int ukprn, int larsCode, string userId, string userDisplayName)
        {
            Ukprn = ukprn;
            LarsCode = larsCode;
            UserId = userId;
            UserDisplayName = userDisplayName;
        }
    }
}
