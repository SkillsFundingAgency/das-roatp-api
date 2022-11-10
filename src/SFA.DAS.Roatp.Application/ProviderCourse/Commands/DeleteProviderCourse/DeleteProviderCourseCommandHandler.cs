using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Domain.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Application.ProviderCourse.Commands.DeleteProviderCourse
{
    public class DeleteProviderCourseCommandHandler : IRequestHandler<DeleteProviderCourseCommand, bool>
    {
        private readonly IProviderCoursesWriteRepository _providerCoursesWriteRepository;
        private readonly ILogger<DeleteProviderCourseCommandHandler> _logger;

        public DeleteProviderCourseCommandHandler(IProviderCoursesWriteRepository providerCoursesWriteRepository,
            ILogger<DeleteProviderCourseCommandHandler> logger)
        {
            _providerCoursesWriteRepository = providerCoursesWriteRepository;
            _logger = logger;
        }

        public async Task<bool> Handle(DeleteProviderCourseCommand command, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Provider course will be deleted for Ukprn:{ukprn} Larscode:{larsCode} by User {userId}", command.Ukprn, command.LarsCode, command.UserId);
            await _providerCoursesWriteRepository.Delete(command.Ukprn, command.LarsCode, command.UserId, command.CorrelationId);
            return true;
        }
    }
}
