﻿using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Application.Mediatr.Responses;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.ProviderCourse.Queries.GetAllProviderCourses
{
    public class GetAllProviderCoursesQueryHandler : IRequestHandler<GetAllProviderCoursesQuery, ValidatedResponse<List<ProviderCourseModel>>>
    {
        private readonly IProviderCoursesReadRepository _providerCoursesReadRepository;
        private readonly IStandardsReadRepository _standardsReadRepository;
        private readonly IProviderCourseLocationsReadRepository _providerCourseLocationsReadRepository;
        private readonly ILogger<GetAllProviderCoursesQueryHandler> _logger;
        public GetAllProviderCoursesQueryHandler(IProviderCoursesReadRepository providerCoursesReadRepository, IStandardsReadRepository standardsReadRepository, ILogger<GetAllProviderCoursesQueryHandler> logger, IProviderCourseLocationsReadRepository providerCourseLocationsReadRepository)
        {
            _providerCoursesReadRepository = providerCoursesReadRepository;
            _standardsReadRepository = standardsReadRepository;
            _providerCourseLocationsReadRepository = providerCourseLocationsReadRepository;
            _logger = logger;
        }

        public async Task<ValidatedResponse<List<ProviderCourseModel>>> Handle(GetAllProviderCoursesQuery request, CancellationToken cancellationToken)
        {
            var providerCourses = await _providerCoursesReadRepository.GetAllProviderCourses(request.Ukprn);

            if (!providerCourses.Any())
            {
                _logger.LogInformation("ProviderCourses data not found for {ukprn}", request.Ukprn);
                return new ValidatedResponse<List<ProviderCourseModel>>(new List<ProviderCourseModel>());
            }

            var standardsLookup = await _standardsReadRepository.GetAllStandards();
            var filteredProviderCourses = FilterExpiredStandards(providerCourses, standardsLookup);

            if (request.ExcludeInvalidCourses)
            {
                filteredProviderCourses = await FilterStandardsWithoutLocations(request.Ukprn, filteredProviderCourses);

                filteredProviderCourses = RemoveUnapprovedRegulatedStandards(filteredProviderCourses);
            }

            var providerCoursesModel = filteredProviderCourses.Select(p => (ProviderCourseModel)p).ToList();

            foreach (var p in providerCoursesModel)
            {
                var course = standardsLookup.Single(c => c.LarsCode == p.LarsCode);
                p.AttachCourseDetails(course.IfateReferenceNumber, course.Level, course.Title, course.Version,
                        course.ApprovalBody);
            }

            return new ValidatedResponse<List<ProviderCourseModel>>(providerCoursesModel);
        }

        private static List<Domain.Entities.ProviderCourse> FilterExpiredStandards(List<Domain.Entities.ProviderCourse> providerCourses, List<Standard> standardsLookup)
        {
            return providerCourses
                .Where(p => standardsLookup.Select(x => x.LarsCode).Contains(p.LarsCode))
                .ToList();
        }

        private async Task<List<Domain.Entities.ProviderCourse>> FilterStandardsWithoutLocations(int ukprn,
            List<Domain.Entities.ProviderCourse> providerCourses)
        {
            List<Domain.Entities.ProviderCourse> filteredCourses = new();
            foreach (var providerCourse in providerCourses)
            {
                var courseLocationsLookup =
                    await _providerCourseLocationsReadRepository.GetAllProviderCourseLocations(ukprn,
                        providerCourse.LarsCode);

                if (courseLocationsLookup != null && courseLocationsLookup.Count != 0)
                {
                    filteredCourses.Add(providerCourse);
                }
            }

            return filteredCourses;
        }

        private static List<Domain.Entities.ProviderCourse> RemoveUnapprovedRegulatedStandards(
            List<Domain.Entities.ProviderCourse> providerCourses)
        {
            providerCourses.RemoveAll(c =>
                c.Standard.IsRegulatedForProvider && !c.IsApprovedByRegulator.GetValueOrDefault());

            return providerCourses;
        }
    }
}
