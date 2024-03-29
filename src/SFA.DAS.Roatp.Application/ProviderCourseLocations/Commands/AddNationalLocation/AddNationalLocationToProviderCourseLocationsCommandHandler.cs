﻿using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Domain.Constants;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Roatp.Domain.Models;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Roatp.Application.Mediatr.Responses;

namespace SFA.DAS.Roatp.Application.ProviderCourseLocations.Commands.AddNationalLocation
{
    public class AddNationalLocationToProviderCourseLocationsCommandHandler : IRequestHandler<AddNationalLocationToProviderCourseLocationsCommand, ValidatedResponse<int>>
    {
    private readonly IProviderLocationsReadRepository _providerLocationsReadRepository;
    private readonly IProviderLocationsWriteRepository _providerLocationsWriteRepository;
    private readonly IProviderCoursesReadRepository _providerCoursesReadRepository;
    private readonly IProviderCourseLocationsWriteRepository _providerCourseLocationsWriteRepository;
    private readonly IProvidersReadRepository _providersReadRepository;
    private readonly ILogger<AddNationalLocationToProviderCourseLocationsCommandHandler> _logger;

    public AddNationalLocationToProviderCourseLocationsCommandHandler(
        IProviderLocationsReadRepository providerLocationsReadRepository,
        IProviderLocationsWriteRepository providerLocationsWriteRepository,
        IProviderCoursesReadRepository providerCoursesReadRepository,
        IProviderCourseLocationsWriteRepository providerCourseLocationsWriteRepository,
        IProvidersReadRepository providersReadRepository,
        ILogger<AddNationalLocationToProviderCourseLocationsCommandHandler> logger)
    {
        _providerLocationsReadRepository = providerLocationsReadRepository;
        _providerLocationsWriteRepository = providerLocationsWriteRepository;
        _providerCoursesReadRepository = providerCoursesReadRepository;
        _providerCourseLocationsWriteRepository = providerCourseLocationsWriteRepository;
        _providersReadRepository = providersReadRepository;
        _logger = logger;
    }

    public async Task<ValidatedResponse<int>> Handle(AddNationalLocationToProviderCourseLocationsCommand request,
        CancellationToken cancellationToken)
    {
        var provider = await _providersReadRepository.GetByUkprn(request.Ukprn);
        var allLocations = await _providerLocationsReadRepository.GetAllProviderLocations(request.Ukprn);
        var nationalLocation = allLocations.SingleOrDefault(l => l.LocationType == LocationType.National);
        if (nationalLocation == null)
        {
            _logger.LogInformation("Creating national location for Ukprn: {ukprn} ", request.Ukprn);
            nationalLocation = ProviderLocation.CreateNationalLocation(provider.Id);
            await _providerLocationsWriteRepository.Create(nationalLocation, request.Ukprn, request.UserId,
                request.UserDisplayName, AuditEventTypes.CreateProviderLocation);
        }

        var providerCourse = await _providerCoursesReadRepository.GetProviderCourse(provider.Id, request.LarsCode);

        var providerCourseLocation = new ProviderCourseLocation
        {
            NavigationId = Guid.NewGuid(),
            ProviderLocationId = nationalLocation.Id,
            ProviderCourseId = providerCourse.Id
        };

        _logger.LogInformation($"Associating national location for ProviderCourse:{providerCourse.Id}");

        var response = await _providerCourseLocationsWriteRepository.Create(providerCourseLocation, request.Ukprn,
            request.UserId, request.UserDisplayName, AuditEventTypes.CreateProviderCourseLocation);

        return new ValidatedResponse<int>(response.Id);
    }
    }
}
