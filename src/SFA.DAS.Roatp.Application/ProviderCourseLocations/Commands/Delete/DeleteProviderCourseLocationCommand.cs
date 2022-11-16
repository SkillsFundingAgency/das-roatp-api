using MediatR;
using SFA.DAS.Roatp.Application.Common;
using System;

namespace SFA.DAS.Roatp.Application.ProviderCourseLocations.Commands.Delete
{
    public class DeleteProviderCourseLocationCommand : IRequest<Unit>, ILarsCode, IUkprn, IUserInfo
    {
        public int Ukprn { get; }
        public int LarsCode { get; }
        public Guid LocationId { get; set; }
        public string UserId { get; }
        public string UserDisplayName { get; }
        public DeleteProviderCourseLocationCommand(int ukprn, int larsCode, Guid id, string userId, string userDisplayName)
        {
            Ukprn = ukprn;
            LarsCode = larsCode;
            LocationId = id;
            UserId = userId;
            UserDisplayName = userDisplayName;
        }
    }
}
