using System.Collections.Generic;

namespace SFA.DAS.Roatp.Jobs.Services
{
    public class CourseDirectoryImportMetrics
    {
        public int ProvidersToLoad { get; set; }
        public int SuccessfulLoads { get; set; }
        public int FailedMappings { get; set; }
        public int FailedLoads { get; set; }
    }
}
