using System;

namespace SFA.DAS.Roatp.Jobs.ApiModels.Lookup
{
    public class Standard
    {
        public string StandardUId { get; set; }
        public int LarsCode { get; set; }
        public string IfateReferenceNumber { get; set; }
        public string Level { get; set; }
        public string Version { get; set; }
        public string Title { get; set; }
        public string ApprovalBody { get; set; }
        public string SectorSubjectAreaTier2Description { get; set; }

        public static implicit operator Domain.Entities.Standard(Standard standard) =>
            new Domain.Entities.Standard
            {
                StandardUId = standard.StandardUId,
                IfateReferenceNumber = standard.IfateReferenceNumber,
                LarsCode = standard.LarsCode,
                Title = standard.Title,
                Version = standard.Version,
                Level = Convert.ToInt32(standard.Level),
                ApprovalBody = string.IsNullOrWhiteSpace(standard.ApprovalBody) ? null : standard.ApprovalBody,
                SectorSubjectArea = standard.SectorSubjectAreaTier2Description
            };
    }
}
