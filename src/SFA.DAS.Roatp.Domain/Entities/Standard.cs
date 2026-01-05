using System.Collections.Generic;
using SFA.DAS.Roatp.Domain.Models;

namespace SFA.DAS.Roatp.Domain.Entities
{
    public class Standard
    {
        public string StandardUId { get; set; }
        public string LarsCode { get; set; }
        public string IfateReferenceNumber { get; set; }
        public int Level { get; set; }
        public string Title { get; set; }
        public string Version { get; set; }
        public string ApprovalBody { get; set; }
        public int SectorSubjectAreaTier1 { get; set; }
        public virtual List<ProviderCourse> ProviderCourses { get; set; }
        public bool IsRegulatedForProvider { get; set; }
        public CourseType CourseType { get; set; }
    }
}
