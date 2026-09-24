using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Domain.Constants;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.ProviderAllowedCourses.Commands.DeleteProviderAllowedCourse;

public class DeleteProviderAllowedCourseCommandHandler(IProviderAllowedCoursesRepository _providerAllowedCoursesRepository, ILogger<DeleteProviderAllowedCourseCommandHandler> _logger) : IRequestHandler<DeleteProviderAllowedCourseCommand>
{
    public async Task Handle(DeleteProviderAllowedCourseCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Hanlde delete provider allowed course for Ukprn:{Ukprn} Larscode:{LarsCode}", command.Ukprn, command.LarsCode);
        await _providerAllowedCoursesRepository.DeleteProviderAllowedCourse(command.Ukprn, command.LarsCode, command.UserId, command.UserDisplayName, AuditEventTypes.DeleteProviderAllowedCourse);
    }
}
