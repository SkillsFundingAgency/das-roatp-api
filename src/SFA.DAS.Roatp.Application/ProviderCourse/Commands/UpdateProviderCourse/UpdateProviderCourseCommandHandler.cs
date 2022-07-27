using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Domain.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Application.ProviderCourse.Commands.UpdateProviderCourse
{
    public class UpdateProviderCourseCommandHandler : IRequestHandler<UpdateProviderCourseCommand, Unit>
    {
        private readonly IProviderCourseEditRepository _providerCourseEditRepository;
        private readonly IProviderCourseReadRepository _providerCourseReadRepository;
        private readonly ILogger<UpdateProviderCourseCommandHandler> _logger;

        public UpdateProviderCourseCommandHandler(IProviderCourseReadRepository providerCourseReadRepository, IProviderCourseEditRepository providerCourseEditRepository, ILogger<UpdateProviderCourseCommandHandler> logger)
        {
            _providerCourseReadRepository = providerCourseReadRepository;
            _providerCourseEditRepository = providerCourseEditRepository;
            _logger = logger;
        }

        public async Task<Unit> Handle(UpdateProviderCourseCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Update provider course details for ukprn: {ukprn} LarsCode: {larscode} by user: {userid}", request.Ukprn, request.LarsCode, request.UserId);
            var providerCourse = await _providerCourseReadRepository.GetProviderCourseByUkprn(request.Ukprn, request.LarsCode);
            if (providerCourse == null)
            {
                _logger.LogError("Provider course not found for ukprn: {ukprn} LarsCode: {larscode}", request.Ukprn, request.LarsCode);
                throw new InvalidOperationException($"Provider course not found for ukprn: {request.Ukprn} LarsCode: {request.LarsCode}");
            }

            providerCourse.ContactUsEmail = request.ContactUsEmail;
            providerCourse.ContactUsPageUrl = request.ContactUsPageUrl;
            providerCourse.ContactUsPhoneNumber = request.ContactUsPhoneNumber;
            providerCourse.StandardInfoUrl = request.StandardInfoUrl;
            providerCourse.IsApprovedByRegulator = request.IsApprovedByRegulator;

            await _providerCourseEditRepository.UpdateProviderCourse(providerCourse);

            return Unit.Value;
        }
    }
}
