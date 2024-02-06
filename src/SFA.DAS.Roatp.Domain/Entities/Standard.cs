﻿using System;

namespace SFA.DAS.Roatp.Domain.Entities
{
    public class Standard
    {
        public string StandardUId { get; set; }
        public int LarsCode { get; set; }
        public string IfateReferenceNumber { get; set; }
        public int Level { get; set; }
        public string Title { get; set; }
        public string Version { get; set; }
        public string ApprovalBody { get; set; }
        [Obsolete]
        public string SectorSubjectArea { get; set; }
        public int SectorSubjectAreaTier1 { get; set; }
    }
}
