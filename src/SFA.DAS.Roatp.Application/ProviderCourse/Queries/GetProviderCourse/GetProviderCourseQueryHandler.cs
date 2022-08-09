﻿using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Application.ProviderCourse.Queries.GetProviderAllCourses;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.ProviderCourse.Queries.GetProviderCourse
{
    public class GetProviderCourseQueryHandler : IRequestHandler<GetProviderCourseQuery, GetProviderCourseQueryResult>
    {
        private readonly IProviderCourseReadRepository _providerCourseReadRepository;
        private readonly IStandardReadRepository _standardReadRepository;
        private readonly ILogger<GetProviderCourseQueryHandler> _logger;

        public GetProviderCourseQueryHandler(IProviderCourseReadRepository providerCourseReadRepository,  IStandardReadRepository standardReadRepository, ILogger<GetProviderCourseQueryHandler> logger)
        {
            _providerCourseReadRepository = providerCourseReadRepository;
            _standardReadRepository = standardReadRepository;
            _logger = logger;
        }

        public async Task<GetProviderCourseQueryResult> Handle(GetProviderCourseQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Getting course for {ukprn} larscode {larscode}",request.Ukprn, request.LarsCode);
            ProviderCourseModel providerCourse = await _providerCourseReadRepository.GetProviderCourseByUkprn(request.Ukprn, request.LarsCode);
            var standardLookup = await _standardReadRepository.GetStandard(request.LarsCode);
            providerCourse.UpdateCourseDetails(standardLookup.IfateReferenceNumber, standardLookup.Level, standardLookup.Title, standardLookup.Version, standardLookup.ApprovalBody);
            return new GetProviderCourseQueryResult { Course = providerCourse };
        }
    }
}