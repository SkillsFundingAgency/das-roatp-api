using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Domain.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Application.ProviderCourseLocations.Commands.Delete
{
    public class DeleteProviderCourseLocationCommandHandler : IRequestHandler<DeleteProviderCourseLocationCommand, Unit>
    {
        private readonly IProviderCourseLocationsDeleteRepository _providerCourseLocationDeleteRepository;
        private readonly ILogger<DeleteProviderCourseLocationCommandHandler> _logger;

        public DeleteProviderCourseLocationCommandHandler(IProviderCourseLocationsDeleteRepository providerCourseLocationDeleteRepository, ILogger<DeleteProviderCourseLocationCommandHandler> logger)
        {
            _providerCourseLocationDeleteRepository = providerCourseLocationDeleteRepository;
            _logger = logger;
        }

        public async Task<Unit> Handle(DeleteProviderCourseLocationCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Deleting provider course location for ukprn: {ukprn} LarsCode: {larscode} providerCourseLocationId: {id} by user: {userid}", request.Ukprn, request.LarsCode, request.Id, request.UserId);
            await _providerCourseLocationDeleteRepository.Delete(request.Id);

            return Unit.Value;
        }
    }
}
