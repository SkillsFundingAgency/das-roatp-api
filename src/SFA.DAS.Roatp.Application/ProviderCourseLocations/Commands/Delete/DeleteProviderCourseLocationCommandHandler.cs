﻿using System.Linq.Expressions;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Domain.Constants;
using SFA.DAS.Roatp.Domain.Interfaces;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Roatp.Application.Mediatr.Responses;

namespace SFA.DAS.Roatp.Application.ProviderCourseLocations.Commands.Delete
{
    public class DeleteProviderCourseLocationCommandHandler : IRequestHandler<DeleteProviderCourseLocationCommand, ValidatedResponse<Unit>>
    {
        private readonly IProviderCourseLocationsWriteRepository _providerCourseLocationsWriteRepository;
        private readonly ILogger<DeleteProviderCourseLocationCommandHandler> _logger;

        public DeleteProviderCourseLocationCommandHandler(IProviderCourseLocationsWriteRepository providerCourseLocationsWriteRepository, ILogger<DeleteProviderCourseLocationCommandHandler> logger)
        {
            _providerCourseLocationsWriteRepository = providerCourseLocationsWriteRepository;
            _logger = logger;
        }

        public async Task<ValidatedResponse<Unit>> Handle(DeleteProviderCourseLocationCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Deleting provider course location for ukprn: {ukprn} LarsCode: {larscode} providerCourseLocationId: {id} by user: {userid}", request.Ukprn, request.LarsCode, request.LocationId, request.UserId);
            await _providerCourseLocationsWriteRepository.Delete(request.LocationId, request.Ukprn, request.UserId, request.UserDisplayName, AuditEventTypes.DeleteProviderCourseLocation);

            return new ValidatedResponse<Unit>(Unit.Value);
        }
    }
}
