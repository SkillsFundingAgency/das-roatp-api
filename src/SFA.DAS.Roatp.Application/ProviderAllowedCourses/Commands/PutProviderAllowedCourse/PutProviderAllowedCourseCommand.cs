using System;
using MediatR;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Application.Mediatr.Responses;

namespace SFA.DAS.Roatp.Application.ProviderAllowedCourses.Commands.PutProviderAllowedCourse;

public class PutProviderAllowedCourseCommand : IRequest<ValidatedResponse<Unit>>, IUserInfo, IUkprn, ILarsCode
{
    public string UserId { get; set; }
    public string UserDisplayName { get; set; }
    public int Ukprn { get; set; }
    public string LarsCode { get; set; }
    public DateTime? LastDateStarts { get; set; }
}
