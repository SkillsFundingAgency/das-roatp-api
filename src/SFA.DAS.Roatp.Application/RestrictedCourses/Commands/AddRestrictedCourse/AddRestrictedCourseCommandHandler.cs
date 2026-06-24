using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Application.Mediatr.Responses;
using SFA.DAS.Roatp.Domain.Constants;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.RestrictedCourses.Commands.AddRestrictedCourse;

public class AddRestrictedCourseCommandHandler(IRestrictedCourseWriteRepository _restrictedCourseWriteRepository, ILogger<AddRestrictedCourseCommandHandler> _logger) : IRequestHandler<AddRestrictedCourseCommand, ValidatedResponse<Unit>>
{
    public async Task<ValidatedResponse<Unit>> Handle(AddRestrictedCourseCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling AddRestrictedCourseCommand for LarsCode: {LarsCode}", command.LarsCode);

        await _restrictedCourseWriteRepository.CreateRestrictedCourse(command.LarsCode, command.UserId, command.UserDisplayName, AuditEventTypes.CreateRestrictedCourse);

        _logger.LogInformation("Created restricted course for LarsCode: {LarsCode}", command.LarsCode);

        return new ValidatedResponse<Unit>(Unit.Value);
    }
}
