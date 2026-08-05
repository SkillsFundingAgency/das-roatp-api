using System;
using MediatR;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Application.Mediatr.Responses;

namespace SFA.DAS.Roatp.Application.ProviderAllowedCourses.Commands.PatchProviderAllowedCourse;

public class PatchProviderAllowedCourseCommand : IRequest<ValidatedResponse<Unit>>, IUserInfo, IUkprn, ILarsCode
{
    public string UserId { get; set; }
    public string UserDisplayName { get; set; }
    public DateTime? LastDateStarts { get; set; }
    public int Ukprn { get; set; }
    public string LarsCode { get; set; }
}
