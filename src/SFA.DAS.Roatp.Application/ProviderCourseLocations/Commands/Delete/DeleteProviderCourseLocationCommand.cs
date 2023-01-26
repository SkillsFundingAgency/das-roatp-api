using MediatR;
using SFA.DAS.Roatp.Application.Common;
using System;
using SFA.DAS.Roatp.Application.Mediatr.Responses;

namespace SFA.DAS.Roatp.Application.ProviderCourseLocations.Commands.Delete
{
    public class DeleteProviderCourseLocationCommand : IRequest<ValidatedResponse<Unit>>, ILarsCodeUkprn, IUkprn, IUserInfo
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
