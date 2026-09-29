using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Roatp.Domain.Constants;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.ProviderAllowedCourses.Commands.DeleteProviderAllowedCourse;

public class DeleteProviderAllowedCourseCommandHandler(IProviderAllowedCoursesRepository _providerAllowedCoursesRepository) : IRequestHandler<DeleteProviderAllowedCourseCommand>
{
    public async Task Handle(DeleteProviderAllowedCourseCommand command, CancellationToken cancellationToken)
    {
        await _providerAllowedCoursesRepository.DeleteProviderAllowedCourse(command.Ukprn, command.LarsCode, command.UserId, command.UserDisplayName, AuditEventTypes.DeleteProviderAllowedCourse, cancellationToken);
    }
}
