using MediatR;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Application.Mediatr.Responses;
using SFA.DAS.Roatp.Domain.Models;

namespace SFA.DAS.Roatp.Application.RestrictedProviders.Commands.RestrictProvider;

public class RestrictProviderCommand : IRequest<ValidatedResponse<Unit>>, IUkprn, IUserInfo
{
    public string UserId { get; set; }
    public string UserDisplayName { get; set; }
    public int Ukprn { get; set; }
    public CourseType CourseType { get; set; }
}
