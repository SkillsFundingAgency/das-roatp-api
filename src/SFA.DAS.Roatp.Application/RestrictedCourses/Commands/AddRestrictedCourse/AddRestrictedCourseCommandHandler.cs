using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Roatp.Application.Mediatr.Responses;
using SFA.DAS.Roatp.Domain.Constants;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.RestrictedCourses.Commands.AddRestrictedCourse;

public class AddRestrictedCourseCommandHandler(IRestrictedCourseWriteRepository _restrictedCourseWriteRepository, IProviderCoursesReadRepository _providerCoursesReadRepository) : IRequestHandler<AddRestrictedCourseCommand, ValidatedResponse<Unit>>
{
    public async Task<ValidatedResponse<Unit>> Handle(AddRestrictedCourseCommand command, CancellationToken cancellationToken)
    {
        var pl = await _providerCoursesReadRepository.GetProviderCourseByLarsCode(command.LarsCode);
        await _restrictedCourseWriteRepository.CreateRestrictedCourse(command.LarsCode, command.UserId, command.UserDisplayName, AuditEventTypes.CreateRestrictedCourse);

        return new ValidatedResponse<Unit>(Unit.Value);
    }
}
