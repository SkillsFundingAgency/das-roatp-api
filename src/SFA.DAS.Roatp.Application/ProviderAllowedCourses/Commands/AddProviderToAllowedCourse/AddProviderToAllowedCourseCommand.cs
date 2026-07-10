using System;
using MediatR;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Application.Mediatr.Responses;

namespace SFA.DAS.Roatp.Application.ProviderAllowedCourses.Commands.AddProviderToAllowedCourse;

public class AddProviderToAllowedCourseCommand : IRequest<ValidatedResponse<Unit>>, IUkprn, ILarsCode, IUserInfo
{
    public string UserId { get; set; }
    public string UserDisplayName { get; set; }
    public DateTime? DateLastStarts { get; set; }
    public int Ukprn { get; set; }
    public string LarsCode { get; set; }
}
