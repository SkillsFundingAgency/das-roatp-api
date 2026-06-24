using MediatR;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Application.Mediatr.Responses;

namespace SFA.DAS.Roatp.Application.RestrictedCourses.Commands.AddRestrictedCourse;

public class AddRestrictedCourseCommand : IRequest<ValidatedResponse<Unit>>, ILarsCode, IUserInfo
{
    public string UserId { get; set; }
    public string UserDisplayName { get; set; }
    public string LarsCode { get; set; }
}
