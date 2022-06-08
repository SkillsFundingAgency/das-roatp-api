using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Domain.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Application.ProviderCourse.Commands.UpdateConfirmRegulatedStandard
{
    public class UpdateConfirmRegulatedStandardCommandHandler : IRequestHandler<UpdateConfirmRegulatedStandardCommand, Unit>
    {
        private readonly IProviderCourseEditRepository _providerCourseEditRepository;
        private readonly IProviderCourseReadRepository _providerCourseReadRepository;
        private readonly ILogger<UpdateConfirmRegulatedStandardCommandHandler> _logger;

        public UpdateConfirmRegulatedStandardCommandHandler(IProviderCourseReadRepository providerCourseReadRepository, IProviderCourseEditRepository providerCourseEditRepository, ILogger<UpdateConfirmRegulatedStandardCommandHandler> logger)
        {
            _providerCourseReadRepository = providerCourseReadRepository;
            _providerCourseEditRepository = providerCourseEditRepository;
            _logger = logger;
        }

        public async Task<Unit> Handle(UpdateConfirmRegulatedStandardCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Update confirm regulated standard for ukprn: {ukprn} LarsCode: {larscode}", request.Ukprn, request.LarsCode);
            var providerCourse = await _providerCourseReadRepository.GetProviderCourseByUkprn(request.Ukprn, request.LarsCode);
            if (providerCourse == null)
            {
                _logger.LogError("Provider course not found for ukprn: {ukprn} LarsCode: {larscode}", request.Ukprn, request.LarsCode);
                throw new InvalidOperationException($"Provider course not found for ukprn: {request.Ukprn} LarsCode: {request.LarsCode}");
            }

            providerCourse.IsApprovedByRegulator = request.IsApprovedByRegulator;
            await _providerCourseEditRepository.UpdateConfirmRegulatedStandard(providerCourse);

            return Unit.Value;
        }
    }
}
