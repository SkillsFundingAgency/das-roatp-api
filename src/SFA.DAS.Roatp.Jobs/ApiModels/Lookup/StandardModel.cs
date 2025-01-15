namespace SFA.DAS.Roatp.Jobs.ApiModels.Lookup;

public class StandardModel
{
    public string StandardUId { get; set; }
    public int LarsCode { get; set; }
    public string IfateReferenceNumber { get; set; }
    public int Level { get; set; }
    public string Version { get; set; }
    public string Title { get; set; }
    public string ApprovalBody { get; set; }
    public int SectorSubjectAreaTier1 { get; set; }

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
            SectorSubjectAreaTier1 = standard.SectorSubjectAreaTier1
        };
}
