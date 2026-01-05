using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Roatp.Domain.Models;

namespace SFA.DAS.Roatp.Application.Standards.Queries.GetAllStandards
{
    public class GetAllStandardsQueryHandler : IRequestHandler<GetAllStandardsQuery, GetAllStandardsQueryResult>
    {
        private readonly IStandardsReadRepository _standardsReadRepository;
        private readonly ILogger<GetAllStandardsQueryHandler> _logger;

        public GetAllStandardsQueryHandler(IStandardsReadRepository standardsReadRepository, ILogger<GetAllStandardsQueryHandler> logger)
        {
            _standardsReadRepository = standardsReadRepository;
            _logger = logger;
        }

        public async Task<GetAllStandardsQueryResult> Handle(GetAllStandardsQuery request, CancellationToken cancellationToken)
        {
            var allStandards = await _standardsReadRepository.GetAllStandards();
            if (request.CourseType.HasValue)
            {
                allStandards = FilterStandardsWithCourseType(allStandards, request.CourseType.Value);
            }
            _logger.LogInformation("Returning {AllStandardsCount} standards", allStandards.Count);
            return new GetAllStandardsQueryResult(allStandards);
        }
        private static List<Standard> FilterStandardsWithCourseType(List<Standard> allStandards, CourseType courseTypeFilter)
        {
            return allStandards
                .Where(c => c.CourseType == courseTypeFilter)
                .ToList();
        }
    }
}
