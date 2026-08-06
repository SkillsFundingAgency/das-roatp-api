using MediatR;
using Microsoft.AspNetCore.JsonPatch;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Application.Mediatr.Responses;
using SFA.DAS.Roatp.Domain.Models;

namespace SFA.DAS.Roatp.Application.ProviderAllowedCourses.Commands.PatchProviderAllowedCourse;

public class PatchProviderAllowedCourseCommand : IRequest<ValidatedResponse<Unit>>, IUserInfo, IUkprn, ILarsCode
{
    public string UserId { get; set; }
    public string UserDisplayName { get; set; }
    public int Ukprn { get; set; }
    public string LarsCode { get; set; }
    public JsonPatchDocument<PatchProviderAllowedCourseModel> PatchDoc { get; set; }
}
