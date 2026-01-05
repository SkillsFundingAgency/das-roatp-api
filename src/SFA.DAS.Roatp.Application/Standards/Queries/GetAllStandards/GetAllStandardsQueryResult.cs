using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Models;

namespace SFA.DAS.Roatp.Application.Standards.Queries.GetAllStandards
{
    public class GetAllStandardsQueryResult
    {
        public List<StandardModel> Standards { get; }

        public GetAllStandardsQueryResult(List<Standard> standards)
        {
            Standards = (standards ?? new List<Standard>())
                .Select(Map)
                .ToList();
        }

        private static StandardModel Map(Standard standard) =>
            new()
            {
                StandardUId = standard.StandardUId,
                LarsCode = standard.LarsCode,
                IfateReferenceNumber = standard.IfateReferenceNumber,
                Level = standard.Level,
                Title = standard.Title,
                ApprovalBody = standard.ApprovalBody,
                IsRegulatedForProvider = standard.IsRegulatedForProvider,
                Route = standard.Route,
                ApprenticeshipType = standard.ApprenticeshipType,
                CourseType = standard.CourseType
            };
    }

    public class StandardModel
    {
        public string StandardUId { get; set; }
        public string LarsCode { get; set; }
        public string IfateReferenceNumber { get; set; }
        public int Level { get; set; }
        public string Title { get; set; }
        public string ApprovalBody { get; set; }
        public bool IsRegulatedForProvider { get; set; }
        public string Route { get; set; }
        public ApprenticeshipType ApprenticeshipType { get; set; }
        public CourseType CourseType { get; set; }
    }

}
