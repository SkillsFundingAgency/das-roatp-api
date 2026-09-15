using System.Collections.Generic;
using MediatR;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Application.Mediatr.Responses;
using SFA.DAS.Roatp.Domain.Models;

namespace SFA.DAS.Roatp.Application.ProviderCourseTypes.Commands.CreateProviderCourseType;

public class CreateProviderCourseTypeCommand : IRequest<ValidatedResponse<Unit>>, IUserInfo
{
    public int Ukprn { get; set; }
    public IEnumerable<CourseType> CourseTypes { get; set; }
    public string UserId { get; set; }
    public string UserDisplayName { get; set; }
}