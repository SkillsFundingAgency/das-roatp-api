using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.ProviderCourse
{
    public class PatchProviderCourseCommandHandler : IRequestHandler<PatchProviderCourseCommand, Unit>
    {
         private readonly IProviderCourseEditRepository _providerCourseEditRepository;
         private readonly IProviderCourseReadRepository _providerCourseReadRepository;
         private readonly ILogger<PatchProviderCourseCommandHandler> _logger;

         public PatchProviderCourseCommandHandler(IProviderCourseEditRepository providerCourseEditRepository, IProviderCourseReadRepository providerCourseReadRepository, ILogger<PatchProviderCourseCommandHandler> logger)
         {
             _providerCourseEditRepository = providerCourseEditRepository;
             _providerCourseReadRepository = providerCourseReadRepository;
             _logger = logger;
         }

        public async Task<Unit> Handle(PatchProviderCourseCommand command, CancellationToken cancellationToken)
        {
            var providerCourse = await
                _providerCourseReadRepository.GetProviderCourseByUkprn(command.Ukprn, command.LarsCode);
            
            if (providerCourse == null)
            {
                _logger.LogError("Provider course not found for ukprn: {ukprn} LarsCode: {larscode}", command.Ukprn, command.LarsCode);
                throw new InvalidOperationException($"Provider course not found for ukprn: {command.Ukprn} LarsCode: {command.LarsCode}");
            }

            var patchedProviderCourse = (Domain.Models.PatchProviderCourse)providerCourse;

            command.Patch.ApplyTo(patchedProviderCourse);
            
            providerCourse.IsApprovedByRegulator = patchedProviderCourse.IsApprovedByRegulator;
            providerCourse.ContactUsEmail = patchedProviderCourse.ContactUsEmail;
            providerCourse.ContactUsPageUrl = patchedProviderCourse.ContactUsPageUrl;
            providerCourse.ContactUsPhoneNumber = patchedProviderCourse.ContactUsPhoneNumber;
            providerCourse.StandardInfoUrl = patchedProviderCourse.StandardInfoUrl;

            await _providerCourseEditRepository.PatchProviderCourse(providerCourse);

            return Unit.Value;
        }
    }
}
