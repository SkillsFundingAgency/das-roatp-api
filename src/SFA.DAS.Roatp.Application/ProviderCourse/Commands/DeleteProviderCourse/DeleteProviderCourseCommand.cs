using MediatR;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Application.Mediatr.Responses;

namespace SFA.DAS.Roatp.Application.ProviderCourse.Commands.DeleteProviderCourse
{
    public class DeleteProviderCourseCommand : IRequest<ValidatedResponse<bool>>, ILarsCodeUkprn, IUkprn, IUserInfo
    {
        public int Ukprn { get; set; }
        public string LarsCode { get; set; }
        public string UserId { get; set; }
        public string UserDisplayName { get; set; }
        public DeleteProviderCourseCommand(int ukprn, string larsCode, string userId, string userDisplayName)
        {
            Ukprn = ukprn;
            LarsCode = larsCode;
            UserId = userId;
            UserDisplayName = userDisplayName;
        }
    }
}
