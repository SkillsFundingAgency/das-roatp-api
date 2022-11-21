using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Domain.Constants;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.ProviderCourse
{
    public class PatchProviderCourseCommandHandler : IRequestHandler<PatchProviderCourseCommand, Unit>
    {
         private readonly IProviderCoursesWriteRepository _providerCoursesWriteRepository;
         private readonly IProviderCoursesReadRepository _providerCoursesReadRepository;
         private readonly ILogger<PatchProviderCourseCommandHandler> _logger;

         public PatchProviderCourseCommandHandler(IProviderCoursesWriteRepository providerCoursesWriteRepository, IProviderCoursesReadRepository providerCoursesReadRepository, ILogger<PatchProviderCourseCommandHandler> logger)
         {
             _providerCoursesWriteRepository = providerCoursesWriteRepository;
             _providerCoursesReadRepository = providerCoursesReadRepository;
             _logger = logger;
         }

        public async Task<Unit> Handle(PatchProviderCourseCommand command, CancellationToken cancellationToken)
        {
            var providerCourse = await
                _providerCoursesReadRepository.GetProviderCourseByUkprn(command.Ukprn, command.LarsCode);
            
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

            await _providerCoursesWriteRepository.PatchProviderCourse(providerCourse, command.Ukprn, command.LarsCode, command.UserId, command.UserDisplayName, AuditEventTypes.UpdateProviderCourseContactDetailsOrConfirmingRegulator);

            return Unit.Value;
        }
    }
}
