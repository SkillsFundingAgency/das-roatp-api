using SFA.DAS.Roatp.Domain.Models;

namespace SFA.DAS.Roatp.Jobs.ApiModels.Lookup;

public class StandardModel
{
    public string StandardUId { get; set; }
    public string LarsCode { get; set; }
    public string IfateReferenceNumber { get; set; }
    public int Level { get; set; }
    public string Version { get; set; }
    public string Title { get; set; }
    public string ApprovalBody { get; set; }
    public int SectorSubjectAreaTier1 { get; set; }
    public bool IsRegulatedForProvider { get; set; }
    public int Duration { get; set; }
    public DurationUnits DurationUnits { get; set; }
    public string Route { get; set; }
    public ApprenticeshipType ApprenticeshipType { get; set; }
    public CourseType CourseType { get; set; }

    public static implicit operator Domain.Entities.Standard(StandardModel standard) =>
        new Domain.Entities.Standard
        {
            StandardUId = standard.StandardUId,
            IfateReferenceNumber = standard.IfateReferenceNumber,
            LarsCode = standard.LarsCode,
            Title = standard.Title,
            Version = standard.Version,
            Level = standard.Level,
            ApprovalBody = string.IsNullOrWhiteSpace(standard.ApprovalBody) ? null : standard.ApprovalBody,
            SectorSubjectAreaTier1 = standard.SectorSubjectAreaTier1,
            IsRegulatedForProvider = standard.IsRegulatedForProvider,
            Duration = standard.Duration,
            DurationUnits = standard.DurationUnits,
            Route = standard.Route,
            ApprenticeshipType = standard.ApprenticeshipType,
            CourseType = standard.CourseType
        };
}
