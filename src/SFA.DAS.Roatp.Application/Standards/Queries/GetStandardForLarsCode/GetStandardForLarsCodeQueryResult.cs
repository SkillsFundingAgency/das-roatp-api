using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Models;

namespace SFA.DAS.Roatp.Application.Standards.Queries.GetStandardForLarsCode;

public class GetStandardForLarsCodeQueryResult
{
    public string StandardUId { get; set; }
    public string LarsCode { get; set; }
    public string IfateReferenceNumber { get; set; }
    public int Level { get; set; }
    public string Title { get; set; }
    public string Version { get; set; }
    public string ApprovalBody { get; set; }
    public bool IsRegulatedForProvider { get; set; }
    public int Duration { get; set; }
    public string DurationUnits { get; set; }
    public string Route { get; set; }
    public ApprenticeshipType ApprenticeshipType { get; set; }
    public CourseType CourseType { get; set; }

    public static implicit operator GetStandardForLarsCodeQueryResult(Standard standard)
    {
        if (standard == null) return null;

        return new GetStandardForLarsCodeQueryResult
        {
            StandardUId = standard.StandardUId,
            LarsCode = standard.LarsCode,
            IfateReferenceNumber = standard.IfateReferenceNumber,
            Level = standard.Level,
            Title = standard.Title,
            Version = standard.Version,
            ApprovalBody = standard.ApprovalBody,
            IsRegulatedForProvider = standard.IsRegulatedForProvider,
            Route = standard.Route,
            Duration = standard.Duration,
            DurationUnits = standard.DurationUnits,
            ApprenticeshipType = standard.ApprenticeshipType,
            CourseType = standard.CourseType
        };
    }
}